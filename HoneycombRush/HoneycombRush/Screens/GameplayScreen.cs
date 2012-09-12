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
        private Texture2D controlstickBoundary;
        private Texture2D controlstick;
        private Texture2D blockTexture;
        private Texture2D smokeButton;
        private Texture2D colisionArea;

        private Vector2 controlstickStartupPosition;
        private Vector2 controlstickBoundaryPosition;
        private Vector2 smokeButtonPosition;
        private Vector2 lastTouchPosition;

        private bool isSmokeButtonClicked;
        private bool drawArrow;
        private bool drawArrowInterval;
        private bool isInMotion;
        private bool isAtStartupCountDown;
        private bool isLevelEnd;
        private bool levelEnded;
        private bool isUserWon;
        private bool userTapToExit;

        private Dictionary<string, Animation> animations;

        private int arrowCounter;

        private Block[,] blocks = new Block[15, 11];

        private TimeSpan gameElapsed;
        private TimeSpan startScreenTime;

        private Bomberman bomberman;

        private DifficultyMode gameDifficultyLevel;

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

            controlstickBoundaryPosition = new Vector2(34, 347);
            smokeButtonPosition = new Vector2(664, 346);
            controlstickStartupPosition = new Vector2(55, 369);

            isInMotion = false;
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

            isSmokeButtonClicked = false;

            // If there was any touch
            if (VirtualThumbsticks.RightThumbstickCenter.HasValue)
            {
                // Button BodyRectangle
                Rectangle buttonRectangle = new Rectangle((int)smokeButtonPosition.X, (int)smokeButtonPosition.Y, smokeButton.Width / 2, smokeButton.Height);

                // Touch BodyRectangle
                Rectangle touchRectangle = new Rectangle((int)VirtualThumbsticks.RightThumbstickCenter.Value.X, (int)VirtualThumbsticks.RightThumbstickCenter.Value.Y, 1, 1);

                // If the touch is in the button
                if (buttonRectangle.Contains(touchRectangle))
                {
                    isSmokeButtonClicked = true;
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

            handleThumbStick();

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
                drawBombButton();

                ScreenManager.SpriteBatch.Draw(controlstickBoundary, controlstickBoundaryPosition, Color.White);
                ScreenManager.SpriteBatch.Draw(controlstick, controlstickStartupPosition, Color.White);

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
            bomberman.ThumbStickArea =
                new Rectangle((int)controlstickBoundaryPosition.X, (int)controlstickBoundaryPosition.Y, controlstickBoundary.Width, controlstickBoundary.Height);

            ScreenManager.Game.Components.Add(bomberman);
        }
        
        /// <summary>
        /// Loads all the necessary textures.
        /// </summary>
        private void loadTextures()
        {
            blockTexture = ScreenManager.Game.Content.Load<Texture2D>("Textures/Block");
            background = ScreenManager.Game.Content.Load<Texture2D>("Textures/Backgrounds/Back");
            controlstickBoundary = ScreenManager.Game.Content.Load<Texture2D>("Textures/controlstickBoundary");
            controlstick = ScreenManager.Game.Content.Load<Texture2D>("Textures/controlstick");
            smokeButton = ScreenManager.Game.Content.Load<Texture2D>("Textures/smokeBtn");
            font16px = ScreenManager.Game.Content.Load<SpriteFont>("Fonts/GameScreenFont16px");
            arrowTexture = ScreenManager.Game.Content.Load<Texture2D>("Textures/arrow");
            font16px = ScreenManager.Game.Content.Load<SpriteFont>("Fonts/GameScreenFont16px");
            font36px = ScreenManager.Game.Content.Load<SpriteFont>("Fonts/GameScreenFont36px");
            colisionArea = ScreenManager.Game.Content.Load<Texture2D>("Textures\\ColisionArea");
        }

        /// <summary>
        /// Handle thumbstick logic
        /// </summary>
        private void handleThumbStick()
        {
            // Calculate the rectangle of the outer circle of the thumbstick
            Rectangle outerControlstick = new Rectangle(
                0,
                (int)controlstickBoundaryPosition.Y - 35,
                controlstickBoundary.Width + 60,
                controlstickBoundary.Height + 60);

            //handle bomb Button
            if (isSmokeButtonClicked)
            {
                dropBomb();
            }

            // Reset the thumbstick position when it is idle
            if (VirtualThumbsticks.LeftThumbstick == Vector2.Zero)
            {
                isInMotion = false;
                bomberman.SetMovement(Vector2.Zero);
                controlstickStartupPosition = new Vector2(55, 369);
            }
            else
            {
                // If not in motion and the touch point is not in the control bounds - there is no movement
                Rectangle touchRectangle = new Rectangle((int)lastTouchPosition.X, (int)lastTouchPosition.Y, 1, 1);

                if (!outerControlstick.Contains(touchRectangle))
                {
                    controlstickStartupPosition = new Vector2(55, 369);
                    isInMotion = false;
                    return;
                }

                // Move the beekeeper
                setMotion();

                // Moves the thumbstick's inner circle
                float radious = controlstick.Width / 2 + 35;
                controlstickStartupPosition = new Vector2(55, 369) + (VirtualThumbsticks.LeftThumbstick * radious);
            }
        }

        /// <summary>
        /// Drops the bomb.
        /// </summary>
        private void dropBomb()
        {
            if (bomberman.Bombs.Count > 0)
            {
                Bomb bomb = bomberman.Bombs.First();
                bomb.SetPosition(new Vector2(bomberman.CollisionArea.X, bomberman.CollisionArea.Y - 10));
                bomb.AnimationDefinitions = animations;
                if (!ScreenManager.Game.Components.Contains(bomb))
                {
                    ScreenManager.Game.Components.Add(bomb);
                    bomberman.Bombs.Remove(bomb);
                }
            }
        }

        /// <summary>
        /// Moves the beekeeper.
        /// </summary>
        private void setMotion()
        {
            Vector2 leftThumbstick = VirtualThumbsticks.LeftThumbstick;

            Vector2 bomberCalculatedPosition = new Vector2(bomberman.CollisionArea.X, bomberman.CollisionArea.Y) + leftThumbstick * 12f;

            if (bomberCalculatedPosition.X < 20 || bomberCalculatedPosition.X + bomberman.CollisionArea.Width > ScreenManager.GraphicsDevice.Viewport.Width)
            {
                leftThumbstick.X = 0;
            }
            if (bomberCalculatedPosition.Y < 20 || bomberCalculatedPosition.Y + bomberman.CollisionArea.Height > ScreenManager.GraphicsDevice.Viewport.Height)
            {
                leftThumbstick.Y = 0;
            }

            if (leftThumbstick == Vector2.Zero)
            {
                isInMotion = false;
            }
            else
            {
                if (bomberman.GetDirection == Bomberman.WalkingDirection.Down || bomberman.GetDirection == Bomberman.WalkingDirection.Up)
                {
                    if (checkBlockCollision(bomberCalculatedPosition))
                    {
                        leftThumbstick.Y = 0;
                        bomberCalculatedPosition = new Vector2(bomberman.CollisionArea.X, bomberman.CollisionArea.Y) + leftThumbstick * 12;

                        if (!checkBlockCollision(bomberCalculatedPosition))
                        {
                            bomberman.SetMovement(leftThumbstick * 12f);
                            isInMotion = true;
                        }
                    }
                }
                else
                {
                    if (checkBlockCollision(bomberCalculatedPosition))
                    {
                        leftThumbstick.X = 0;
                        bomberCalculatedPosition = new Vector2(bomberman.CollisionArea.X, bomberman.CollisionArea.Y) + leftThumbstick * 12;

                        if (!checkBlockCollision(bomberCalculatedPosition))
                        {
                            bomberman.SetMovement(leftThumbstick * 12f);
                            isInMotion = true;
                        }
                    }
                }

                bomberCalculatedPosition = new Vector2(bomberman.CollisionArea.X, bomberman.CollisionArea.Y) + leftThumbstick * 12;

                if (!checkBlockCollision(bomberCalculatedPosition))
                {
                    bomberman.SetMovement(leftThumbstick * 12f);
                    isInMotion = true;
                }
            }
        }

        /// <summary>
        /// Checks if the beekeeper collides with a Block.
        /// </summary>
        /// <param name="bomberCalculatedPosition">The beekeeper's position.</param>
        /// <returns>True if the beekeeper collides with a Block and false otherwise.</returns>
        private bool checkBlockCollision(Vector2 bomberCalculatedPosition)
        {
            // We do not use the beekeeper's collision area property as he has not actually moved at this point and
            // is still in his previous position
            Rectangle bomberTempCollisionArea = new Rectangle(
                (int)bomberCalculatedPosition.X,
                (int)bomberCalculatedPosition.Y,
                bomberman.CollisionArea.Width,
                bomberman.CollisionArea.Height);

            foreach (Block block in blocks)
            {
                if (block != null && bomberTempCollisionArea.Intersects(block.CollisionArea))
                {
                    // TODO move bomber out of collision area.
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Checks whether the current game is over, and if so performs the necessary actions.
        /// </summary>
        /// <returns>True if the current game is over and false otherwise.</returns>
        private bool checkIfCurrentGameFinished()
        {
            levelEnded = false;
            //isUserWon = vat.CurrentVatCapacity >= vat.MaxVatCapacity;

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
                        // If is in high score, gets is name
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
        /// Draws the smoke button.
        /// </summary>
        private void drawBombButton()
        {
            if (isSmokeButtonClicked)
            {
                ScreenManager.SpriteBatch.Draw(
                   smokeButton, new Rectangle((int)smokeButtonPosition.X, (int)smokeButtonPosition.Y, 109, 109),
                   new Rectangle(109, 0, 109, 109), Color.White);
            }
            else
            {
                ScreenManager.SpriteBatch.Draw(
                smokeButton, new Rectangle((int)smokeButtonPosition.X, (int)smokeButtonPosition.Y, 109, 109),
                new Rectangle(0, 0, 109, 109), Color.White);
            }
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
