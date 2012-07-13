using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Boom.Logic
{
    public class Player
    {
        private Vector2 position;
        private IAnimation playerAnimation;
        
        /// <summary>
        /// Gets the width of the player ship
        /// </summary>
        public int Width
        {
            get { return playerAnimation.FrameWidth; }
        }

        /// <summary>
        /// Gets the height of the player ship
        /// </summary>
        public int Height
        {
            get { return playerAnimation.FrameHeight; }
        }

        /// <summary>
        /// Gets or sets the X position.
        /// </summary>
        /// <value>
        /// The X position.
        /// </value>
        public float X
        {
            get
            {
                return position.X;
            }
            set
            {
                position.X = value;
            }
        }

        /// <summary>
        /// Gets or sets the Y position.
        /// </summary>
        /// <value>
        /// The Y position.
        /// </value>
        public float Y
        {
            get
            {
                return position.Y;
            }
            set
            {
                position.Y = value;
            }
        }

        public bool IsActive
        {
            get
            {
                return playerAnimation.IsActive;
            }
            set
            {
                playerAnimation.IsActive = value;
            }
        }

        /// <summary>
        /// Initializes the specified player.
        /// </summary>
        /// <param name="animation">The animation.</param>
        /// <param name="defPosition">The default defPosition.</param>
        public void Initialize(IAnimation animation, Vector2 defPosition)
        {
            playerAnimation = animation;
            this.position = defPosition;
        }

        public void Update()
        {
            playerAnimation.Position = position;
            playerAnimation.Update();
        }

        /// <summary>
        /// Draw the player
        /// </summary>
        /// <param name="spriteBatch">The sprite batch.</param>
        public void Draw(SpriteBatch spriteBatch)
        {
            playerAnimation.Draw(spriteBatch);
        }
    }
}