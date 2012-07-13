using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Boom.Logic
{
    public interface IAnimation
    {
        /// <summary>
        /// Gets or sets the width of the frame.
        /// </summary>
        /// <value>
        /// The width of the frame.
        /// </value>
        int FrameWidth { get; set; }

        /// <summary>
        /// Gets or sets the height of the frame.
        /// </summary>
        /// <value>
        /// The height of the frame.
        /// </value>
        int FrameHeight { get; set; }

        /// <summary>
        /// Gets or sets the position.
        /// </summary>
        /// <value>
        /// The position.
        /// </value>
        Vector2 Position { get; set; }

        bool IsActive { get; set; }

        void Update();

        void Draw(SpriteBatch spriteBatch);
    }
}