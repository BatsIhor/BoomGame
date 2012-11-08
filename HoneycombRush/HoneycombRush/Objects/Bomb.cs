using System;
using HoneycombRush.Logic;
using HoneycombRush.Screens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HoneycombRush.Objects
{
    public class Bomb : TexturedDrawableGameComponent
    {
        private const string BOMB_ANIMATION = "BombAnimation";

        private Vector2 bodySize = new Vector2(36, 37);
        private bool isExploded;
        private bool isStarted;
        private TimeSpan timer;

        public Bomb(Game game, GameplayScreen gamePlayScreen, Vector2 position)
            : base(game, gamePlayScreen)
        {
            this.Position = position;
            DrawOrder = (int) position.Y;
        }

        public override Rectangle BodyRectangle
        {
            get { return new Rectangle((int) Position.X, (int) Position.Y, (int) bodySize.X, (int) bodySize.Y); }
        }

        public override Rectangle CollisionArea
        {
            get { return new Rectangle((int) Position.X, (int) Position.Y, (int) bodySize.X, (int) bodySize.Y); }
        }

        internal bool IsExploded
        {
            get { return isExploded; }
        }

        /// <summary>
        /// Initialize the .
        /// </summary>
        public override void Initialize()
        {
            // Initialize the animation 
            AnimationDefinitions[BOMB_ANIMATION].PlayFromFrameIndex(0);

            base.Initialize();
        }

        /// <summary>
        /// Updates the Block's status.
        /// </summary>
        /// <param name="gameTime">Game time information.</param>
        public override void Update(GameTime gameTime)
        {
            if (!GamePlayScreen.IsActive)
            {
                base.Update(gameTime);
                return;
            }

            if (isStarted)
            {
                timer -= gameTime.ElapsedGameTime;

                if (timer.Seconds == 0)
                {
                    //TODO set new animation. 
                    AudioManager.PlaySound("Explosion", false, .10f);
                    isExploded = true;
                }
            }

            AnimationDefinitions[BOMB_ANIMATION].Update(gameTime, true);

            base.Update(gameTime);
        }

        /// <summary>
        /// Render the Block.
        /// </summary>
        /// <param name="gameTime">Game time information.</param>
        public override void Draw(GameTime gameTime)
        {
            if (!GamePlayScreen.IsActive)
            {
                base.Draw(gameTime);
                return;
            }

            SpriteBatch.Begin();
            AnimationDefinitions[BOMB_ANIMATION].Draw(SpriteBatch, Position, SpriteEffects.None);
            SpriteBatch.End();

            base.Draw(gameTime);
        }

        public void SetPosition(Vector2 position)
        {
            this.Position = position;
        }

        internal void Start()
        {
            isStarted = true;
            timer = TimeSpan.FromSeconds(3);
        }

        internal void Stop()
        {
            isStarted = false;
            isExploded = false;
        }
    }
}