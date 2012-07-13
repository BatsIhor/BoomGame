using System;
using System.Collections.Generic;

using HoneycombRush.Logic;
using HoneycombRush.Screens;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HoneycombRush.Objects
{
    /// <summary>
    /// Represents the beekeeper, the player's avatar.
    /// </summary>
    public class Bomberman : TexturedDrawableGameComponent
    {
        #region Enums

        /// <summary>
        /// Represents the direction in which the beekeeper is walking.
        /// </summary>
        public enum WalkingDirection
        {
            Down = 0,
            Up = 10,
            Left = 20,
            Right = 30,
        }

        #endregion

        #region Fields/Properties

        private const string BOMBER_ANIMATION_KEY = "BomberAnimation";

        public Texture2D ColisionAreaRect { get; set; }
        private Vector2 bodySize = new Vector2(43, 63);
        private Vector2 velocity;
        private WalkingDirection direction = WalkingDirection.Up;
        private int lastFrameCounter;

        /// <summary>
        /// Represents the bounds of the component.
        /// </summary>
        public override Rectangle BodyRectangle
        {
            get
            {
                return new Rectangle((int)FramePosition.X - 5, (int)FramePosition.Y + 5, (int)bodySize.X, (int)bodySize.Y);
            }
        }

        /// <summary>
        /// Represents an area used for collision calculations.
        /// </summary>
        public override Rectangle CollisionArea
        {
            get
            {
                return new Rectangle((int)FramePosition.X, (int)FramePosition.Y + 40, (int)bodySize.X - 10, (int)bodySize.Y - 40);
            }
        }

        public Vector2 FramePosition
        {
            get { return new Vector2(Position.X, Position.Y); }
        }

        public Rectangle ThumbStickArea { get; set; }

        public bool IsInMotion { get; set; }

        public List<Bomb> Bombs { get; set; }
        
        #endregion

        #region Initialization

        /// <summary>
        /// Creates a new beekeeper instance.
        /// </summary>
        /// <param name="game">The game object.</param>
        /// <param name="gamePlayScreen">The gameplay screen.</param>
        public Bomberman(Game game, GameplayScreen gamePlayScreen, Vector2 position)
            : base(game, gamePlayScreen)
        {
            this.Position = position;
        }

        /// <summary>
        /// Initialize the beekepper.
        /// </summary>
        public override void Initialize()
        {
            // Initialize the animation 
            AnimationDefinitions[BOMBER_ANIMATION_KEY].PlayFromFrameIndex(0);

            base.Initialize();
        }

        #endregion

        /// <summary>
        /// Updates the beekeeper's status.
        /// </summary>
        /// <param name="gameTime">Game time information</param>
        public override void Update(GameTime gameTime)
        {
            if (!(GamePlayScreen.IsActive && GamePlayScreen.IsStarted))
            {
                base.Update(gameTime);
                return;
            }

            AnimationDefinitions[BOMBER_ANIMATION_KEY].Update(gameTime, IsInMotion);

            base.Update(gameTime);
        }

        /// <summary>
        /// Renders the beekeeper.
        /// </summary>
        /// <param name="gameTime">Game Time obj </param>
        public override void Draw(GameTime gameTime)
        {
            if (!(GamePlayScreen.IsActive && GamePlayScreen.IsStarted))
            {
                base.Draw(gameTime);
                return;
            }

            SpriteBatch.Begin();

            bool hadDirectionChanged = false;
            WalkingDirection tempDirection = direction;

            determineDirection(ref tempDirection);

            // Indicate the direction has changed
            if (tempDirection != direction)
            {
                hadDirectionChanged = true;
                direction = tempDirection;
            }

            if (hadDirectionChanged)
            {
                // Update the animation
                lastFrameCounter = 0;
                AnimationDefinitions[BOMBER_ANIMATION_KEY].PlayFromFrameIndex(lastFrameCounter + (int)direction);
            }
            else
            {
                // Because our animation is 10 cells, and the row is 10 cells,
                // we need to reset the counter after 10 rounds

                if (lastFrameCounter == 10)
                {
                    lastFrameCounter = 0;
                    AnimationDefinitions[BOMBER_ANIMATION_KEY].PlayFromFrameIndex(lastFrameCounter + (int)direction);
                }
                else
                {
                    lastFrameCounter++;
                }
            }

            AnimationDefinitions[BOMBER_ANIMATION_KEY].Draw(SpriteBatch, FramePosition, SpriteEffects.None, new Vector2(20, 5), ColisionAreaRect);

            SpriteBatch.End();

            base.Draw(gameTime);
        }

        /// <summary>
        /// Updates the beekeeper's position.
        /// </summary>
        /// <param name="movement">A vector which contains the desired adjustment to 
        /// the beekeeper's position.</param>
        public void SetMovement(Vector2 movement)
        {
            velocity = movement;

            if (Math.Abs(velocity.X) > Math.Abs(velocity.Y))
            {
                Position.X += velocity.X;
            }
            else
            {
                Position.Y += velocity.Y;
            }
        }

        public WalkingDirection GetDirection
        {
            get
            {

                return direction;
            }
        }

        /// <summary>
        /// Returns movement information according to the current virtual thumbstick input.
        /// </summary>
        /// <param name="tempDirection">
        /// Enum describing the inpot direction.
        /// </param>
        private void determineDirection(ref WalkingDirection tempDirection)
        {
            if (!VirtualThumbsticks.LeftThumbstickCenter.HasValue)
            {
                return;
            }

            Rectangle touchPointRectangle = new Rectangle((int)VirtualThumbsticks.LeftThumbstickCenter.Value.X, (int)VirtualThumbsticks.LeftThumbstickCenter.Value.Y, 1, 1);

            if (ThumbStickArea.Intersects(touchPointRectangle))
            {
                if (Math.Abs(VirtualThumbsticks.LeftThumbstick.X) > Math.Abs(VirtualThumbsticks.LeftThumbstick.Y))
                {
                    tempDirection = VirtualThumbsticks.LeftThumbstick.X > 0 ? WalkingDirection.Right : WalkingDirection.Left;
                }
                else
                {
                    tempDirection = VirtualThumbsticks.LeftThumbstick.Y > 0 ? WalkingDirection.Down : WalkingDirection.Up;
                }
            }
        }
    }
}
