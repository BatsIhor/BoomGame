#region File Description
//-----------------------------------------------------------------------------
// GameplayScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml.Linq;

using HoneycombRush.Logic;
using HoneycombRush.Objects;
using HoneycombRush.ScreenManagerLogic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;

namespace HoneycombRush.Screens
{
    public class GameplayScreen : GameScreen
    {
        #region Fields

        private const string SmokeText = "Smoke";

        private SpriteFont font16px;
        private SpriteFont font36px;

        private Texture2D arrowTexture;
        private Texture2D background;
        private Texture2D blockTexture;
        
        private Texture2D colisionArea;

        private Vector2 smokeButtonPosition;
        private Vector2 lastTouchPosition;

        private bool isAtStartupCountDown;
        private bool isLevelEnd;
        private bool levelEnded;
        private bool isUserWon;
        private bool userTapToExit;

        private Dictionary<string, Animation> animations;
        private Block[,] blocks = new Block[15, 11];
        private TimeSpan gameElapsed;
        private TimeSpan startScreenTime;
        private Bomberman bomberman;
        private DifficultyMode gameDifficultyLevel;
        private ThumbStickLogic thumbStickLogic;

        #endregion

        #region Properties

        public bool IsStarted
        {
            get
            {
                return !isAtStartupCountDown && !levelEnded;
            }
        }

        private int score
        {
            get
            {
                int highscoreFactor = ConfigurationManager.ModesConfiguration[gameDifficultyLevel].HighScoreFactor;

                return highscoreFactor * (int)gameElapsed.TotalMilliseconds;
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new gameplay screen.
        /// </summary>
        /// <param name="gameDifficultyMode">The desired game difficulty.</param>
        public GameplayScreen(DifficultyMode gameDifficultyMode)
        {
            TransitionOnTime = TimeSpan.FromSeconds(0.0);
            TransitionOffTime = TimeSpan.FromSeconds(0.0);
            startScreenTime = TimeSpan.FromSeconds(3);

            //Loads configuration
            ConfigurationManager.LoadConfiguration(XDocument.Load("Content/Configuration/Configuration.xml"));
            ConfigurationManager.DifficultyMode = gameDifficultyMode;

            gameDifficultyLevel = gameDifficultyMode;
            gameElapsed = ConfigurationManager.ModesConfiguration[gameDifficultyLevel].GameElapsed;

            smokeButtonPosition = new Vector2(664, 346);

            isAtStartupCountDown = true;
            isLevelEnd = false;

            EnabledGestures = GestureType.Tap;
        }

        #endregion

        #region Loading and Unloading

        /// <summary>
        /// Loads content and assets.
        /// </summary>
        public void LoadAssets()
        {
            thumbStickLogic = new ThumbStickLogic(ScreenManager);
            animations = XmlLogic.LoadAnimationFromXml(ScreenManager);

            loadTextures();

            createGameComponents();

            AudioManager.PlayMusic("InGameSong_Loop");
        }

        /// <summary>
        /// Unloads game components which are no longer needed once the game ends.
        /// </summary>
        public override void UnloadContent()
        {
            var componentList = ScreenManager.Game.Components;

            for (int index = 0; index < componentList.Count; index++)
            {
                if (componentList[index] != this && componentList[index] != ScreenManager &&
                    !(componentList[index] is AudioManager))
                {
                    componentList.RemoveAt(index);
                    index--;
                }
            }

            base.UnloadContent();
        }

        #endregion

        InputState input;

        /// <summary>
        /// Handle the player's input.
        /// </summary>
        /// <param name="gameTime">
        /// The game Time.
        /// </param>
        /// <param name="input">
        /// gamepad input state
        /// </param>
        public override void HandleInput(GameTime gameTime, InputState input)
        {
            if (IsActive)
            {
                if (input == null)
                {
                    throw new ArgumentNullException("input");
                }

                this.input = input;
                VirtualThumbsticks.Update(input);

                if (input.IsPauseGame(null))
                {
                    pauseCurrentGame();
                }
            }
            if (input.TouchState.Count > 0)
            {
                foreach (TouchLocation touch in input.TouchState)
                {
                    lastTouchPosition = touch.Position;
                }
            }

            if (input.Gestures.Count > 0)
            {
                if (isLevelEnd)
                {
                    if (input.Gestures[0].GestureType == GestureType.Tap)
                    {
                        userTapToExit = true;
                    }
                }
            }
        }

        /// <summary>
        /// Perform the game's update logic.
        /// </summary>
        /// <param name="gameTime">Game time information.</param>
        /// <param name="otherScreenHasFocus">Whether or not another screen currently has the focus.</param>
        /// <param name="coveredByOtherScreen">Whether or not this screen is covered by another.</param>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            // When the game starts the first thing the user sees is the count down before the game actually begins
            if (isAtStartupCountDown)
            {
                startScreenTime -= gameTime.ElapsedGameTime;
            }

            // Check for and handle a game over
            if (checkIfCurrentGameFinished())
            {
                base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
                return;
            }

            if (!(IsActive && IsStarted))
            {
                base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
                return;
            }

            gameElapsed -= gameTime.ElapsedGameTime;

            thumbStickLogic.HandleThumbStick(blocks, bomberman, input);

            bomberman.DrawOrder = 100;

            // We want to determine the draw order of the beekeeper,
            // if the beekeeper is under half the height of the Block 
            // it should be drawn over the Block.
            foreach (Block block in blocks)
            {
                if (block != null)
                {
                    bomberman.DrawOrder = Math.Max(bomberman.DrawOrder, block.BodyRectangle.Y + 1);
                }
            }

            if (gameElapsed.Minutes == 0 && gameElapsed.Seconds == 10)
            {
                AudioManager.PlaySound("10SecondCountDown");
            }

            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
        }

        /// <summary>
        /// Draw the game screen.
        /// </summary>
        /// <param name="gameTime">Game time information.</param>
        public override void Draw(GameTime gameTime)
        {
            ScreenManager.SpriteBatch.Begin();

            // Draw count down screen
            if (isAtStartupCountDown)
            {
                drawStartupString();
            }

            if (IsActive && IsStarted)
            {
                thumbStickLogic.Draw(ScreenManager);

                ScreenManager.SpriteBatch.DrawString(font16px, SmokeText, new Vector2(684, 456), Color.White);
            }

            drawLevelEndIfNecessary();

            ScreenManager.SpriteBatch.End();

            base.Draw(gameTime);
        }

        #region Private Methods

        /// <summary>
        /// If the level is over, draws text describing the level's outocme.
        /// </summary>
        private void drawLevelEndIfNecessary()
        {
            if (isLevelEnd)
            {
                string stringToDisplay = string.Empty;

                if (isUserWon)
                {
                    stringToDisplay = "You Win!";
                }
                else
                {
                    stringToDisplay = "Time Is Up!";
                }

                var stringVector = font36px.MeasureString(stringToDisplay);

                ScreenManager.SpriteBatch.DrawString(
                   font36px,
                   stringToDisplay,
                   new Vector2(ScreenManager.GraphicsDevice.Viewport.Width / 2 - stringVector.X / 2, ScreenManager.GraphicsDevice.Viewport.Height / 2 - stringVector.Y / 2),
                   Color.White);
            }
        }

        /// <summary>
        /// Advances to the next screen based on the current difficulty and whether or not the user has won.
        /// </summary>
        /// <param name="isWon">Whether or not the user has won the current level.</param>
        private void moveToNextScreen(bool isWon)
        {
            ScreenManager.AddScreen(new BackgroundScreen("pauseBackground"), null);

            if (isWon)
            {
                switch (gameDifficultyLevel)
                {
                    case DifficultyMode.Easy:
                    case DifficultyMode.Medium:
                        ScreenManager.AddScreen(
                               new LevelOverScreen("You Finished Level: " + gameDifficultyLevel.ToString(), ++gameDifficultyLevel), null);
                        break;
                    case DifficultyMode.Hard:
                        ScreenManager.AddScreen(new LevelOverScreen("You Win", null), null);
                        break;
                }
            }
            else
            {
                ScreenManager.AddScreen(new LevelOverScreen("You Lose", null), null);
            }

            AudioManager.StopMusic();
            AudioManager.StopSound("BeeBuzzing_Loop");
        }

        /// <summary>
        /// Pause the game.
        /// </summary>
        private void pauseCurrentGame()
        {
            // Pause sounds
            AudioManager.PauseResumeSounds(false);

            // Set pause screen
            ScreenManager.AddScreen(new BackgroundScreen("pauseBackground"), null);
            ScreenManager.AddScreen(new PauseScreen(), null);
        }

        /// <summary>
        /// Create all the game components.
        /// </summary>
        private void createGameComponents()
        {
            // Create all the blocks and the bees
            blocks = XmlLogic.CreateLevel(ScreenManager, blockTexture, animations, this);

            // Creates the Bomberman
            bomberman = new Bomberman(ScreenManager.Game, this, new Vector2(20, 20));
            bomberman.Bombs = new List<Bomb> { new Bomb(ScreenManager.Game, this, Vector2.Zero) };
            bomberman.AnimationDefinitions = animations;
            bomberman.ColisionAreaRect = colisionArea;
            bomberman.ThumbStickArea = thumbStickLogic.GetThumbStickArea();


            ScreenManager.Game.Components.Add(bomberman);
        }

        /// <summary>
        /// Loads all the necessary textures.
        /// </summary>
        private void loadTextures()
        {
            thumbStickLogic.LoadTextures(ScreenManager);

            blockTexture = ScreenManager.Game.Content.Load<Texture2D>("Textures/Block");
            background = ScreenManager.Game.Content.Load<Texture2D>("Textures/Backgrounds/Back");            
            font16px = ScreenManager.Game.Content.Load<SpriteFont>("Fonts/GameScreenFont16px");
            arrowTexture = ScreenManager.Game.Content.Load<Texture2D>("Textures/arrow");
            font16px = ScreenManager.Game.Content.Load<SpriteFont>("Fonts/GameScreenFont16px");
            font36px = ScreenManager.Game.Content.Load<SpriteFont>("Fonts/GameScreenFont36px");
            colisionArea = ScreenManager.Game.Content.Load<Texture2D>("Textures\\ColisionArea");
        }

        /// <summary>
        /// Checks whether the current game is over, and if so performs the necessary actions.
        /// </summary>
        /// <returns>True if the current game is over and false otherwise.</returns>
        private bool checkIfCurrentGameFinished()
        {
            levelEnded = false;

            // If the vat is full, the player wins
            if (isUserWon || gameElapsed <= TimeSpan.Zero)
            {
                levelEnded = true;
            }

            // if true, game is over
            if (gameElapsed <= TimeSpan.Zero || levelEnded)
            {
                isLevelEnd = true;
                if (userTapToExit)
                {
                    ScreenManager.RemoveScreen(this);

                    if (isUserWon) // True - the user won
                    {
                        AudioManager.PlaySound("Victory");
                    }
                    else
                    {
                        AudioManager.PlaySound("Defeat");
                    }

                    moveToNextScreen(isUserWon);
                }
            }

            return false;
        }

        /// <summary>
        /// Draws the count down string.
        /// </summary>
        private void drawStartupString()
        {
            // If needed
            if (isAtStartupCountDown)
            {
                string text = string.Empty;

                // If countdown is done
                if (startScreenTime.Seconds == 0)
                {
                    text = "Go!";
                    isAtStartupCountDown = false;
                    AudioManager.PlaySound("BeeBuzzing_Loop", true, .6f);
                }
                else
                {
                    text = startScreenTime.Seconds.ToString();
                }

                Vector2 size = font16px.MeasureString(text);

                Vector2 textPosition = (new Vector2(ScreenManager.GraphicsDevice.Viewport.Width,
                     ScreenManager.GraphicsDevice.Viewport.Height) - size) / 2f;

                ScreenManager.SpriteBatch.DrawString(font36px, text, textPosition, Color.White);
            }
        }

        #endregion
    }
}
