using System;

using HoneycombRush.Screens;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HoneycombRush.Objects
{
    /// <summary>
    /// Repesent the base Bomb component.
    /// </summary>
    public abstract class BombBase : TexturedDrawableGameComponent
    {
        protected string AnimationKey { get; set; }

        protected abstract int MaxVelocity { get; }
        protected abstract float AccelerationFactor { get; }
        
        public override Rectangle BodyRectangle
        {
            get
            {
                if (Texture == null)
                {
                    return default(Rectangle);
                }
                else
                {
                    // The bee's texture is an animation strip, so we must devide the texture's width by three 
                    // to get the bee's actual width
                    return new Rectangle((int)Position.X, (int)Position.Y, Texture.Width / 3, Texture.Height);
                }
            }
        }

        /// <summary>
        /// Creates a new bee instance.
        /// </summary>
        /// <param name="game">
        /// The game object.
        /// </param>
        /// <param name="gamePlayScreen">
        /// The gameplay screen.
        /// </param>
        public BombBase(Game game, GameplayScreen gamePlayScreen)
            : base(game, gamePlayScreen)
        {
            DrawOrder = Int32.MaxValue - 20;
        }

        /// <summary>
        /// Initialize the bee's location and animation.
        /// </summary>
        public override void Initialize()
        {
            // Start up position
            SetStartupPosition();
            if (!string.IsNullOrEmpty(AnimationKey))
            {
                AnimationDefinitions[AnimationKey].PlayFromFrameIndex(0);
            }
            base.Initialize();
        }

        /// <summary>
        /// Updates the bee's status.
        /// </summary>
        /// <param name="gameTime">Game time information.</param>
        public override void Update(GameTime gameTime)
        {
            if (!(GamePlayScreen.IsActive && GamePlayScreen.IsStarted))
            {
                base.Update(gameTime);
                return;
            }

            if (!string.IsNullOrEmpty(AnimationKey))
            {
                AnimationDefinitions[AnimationKey].Update(gameTime, true);
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// Renders the bee.
        /// </summary>
        /// <param name="gameTime">Game time information.</param>
        public override void Draw(GameTime gameTime)
        {
            if (GamePlayScreen.IsActive && GamePlayScreen.IsStarted)
            {
                SpriteBatch.Begin();

                // If the bee has an animation, draw it
                if (!string.IsNullOrEmpty(AnimationKey))
                {
                    AnimationDefinitions[AnimationKey].Draw(SpriteBatch, Position, SpriteEffects.None);
                }
                else
                {
                    SpriteBatch.Draw(Texture, Position, null, Color.White, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);
                }

                SpriteBatch.End();
            }

            base.Draw(gameTime);
        }

        /// <summary>
        /// Sets the startup position for the bee.
        /// </summary>
        public virtual void SetStartupPosition()
        {
        }
    }
}
