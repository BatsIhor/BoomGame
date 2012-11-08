using HoneycombRush.Screens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HoneycombRush.Objects
{
    /// <summary>
    /// Represent a single Block
    /// </summary>
    public class Block : TexturedDrawableGameComponent
    {
        #region Fields/Properties

        private Vector2 bodySize = new Vector2(40, 36);

        public override Rectangle BodyRectangle
        {
            get { return new Rectangle((int) Position.X, (int) Position.Y, (int) bodySize.X, (int) bodySize.Y); }
        }

        public override Rectangle CollisionArea
        {
            get { return new Rectangle((int) Position.X, (int) Position.Y, (int) bodySize.X, (int) bodySize.Y); }
        }

        #endregion

        #region Initialization

        /// <summary>
        /// Creates a new Block instance.
        /// </summary>
        /// <param name="game">The game object.</param>
        /// <param name="gamePlayScreen">The gameplay screen.</param>
        /// <param name="texture">The texture representing the Block.</param>
        /// <param name="position">The Block's position.</param>
        public Block(Game game, GameplayScreen gamePlayScreen, Texture2D texture, Vector2 position)
            : base(game, gamePlayScreen)
        {
            this.Texture = texture;
            this.Position = position;

            DrawOrder = (int) position.Y;
        }

        #endregion

        #region Update

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

            base.Update(gameTime);
        }

        #endregion

        #region Render

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
            SpriteBatch.Draw(Texture, Position, Color.White);
            SpriteBatch.End();

            base.Draw(gameTime);
        }

        #endregion
    }
}