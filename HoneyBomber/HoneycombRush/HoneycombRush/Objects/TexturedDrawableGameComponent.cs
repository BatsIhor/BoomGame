using System.Collections.Generic;

using HoneycombRush.Logic;
using HoneycombRush.Screens;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HoneycombRush.Objects
{
    /// <summary>
    /// This abstract class represent a component which has a texture that represents it visually.
    /// </summary>
    public abstract class TexturedDrawableGameComponent : DrawableGameComponent
    {
        #region Fields/Properties

        protected Vector2 Position;
        protected Texture2D Texture { get; set; }
        protected GameplayScreen GamePlayScreen { get; set; }
        protected SpriteBatch SpriteBatch { get; set; }

        /// <summary>
        /// Represents the bounds of the component.
        /// </summary>
        public virtual Rectangle BodyRectangle
        {
            get
            {
                if (Texture == null)
                {
                    return default(Rectangle);
                }
                else
                {
                    return new Rectangle((int)Position.X, (int)Position.Y, Texture.Width, Texture.Height);
                }
            }
        }

        /// <summary>
        /// Represents an area used for collision calculations.
        /// </summary>
        public virtual Rectangle CollisionArea
        {
            get
            {
                return default(Rectangle);
            }
        }

        public Dictionary<string, Animation> AnimationDefinitions { get; set; }


        #endregion
        
        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="game">Associated game object.</param>
        /// <param name="gamePlayScreen">Gameplay screen where the component will be presented.</param>
        public TexturedDrawableGameComponent(Game game, GameplayScreen gamePlayScreen)
            : base(game)
        {
            this.GamePlayScreen = gamePlayScreen;

            SpriteBatch = (SpriteBatch)Game.Services.GetService(typeof(SpriteBatch));
        }
    }
}
