using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Boom.Logic
{
    public class Animation : IAnimation
    {
        private Texture2D spriteStrip;
        private float scale;
        private int frameCount;
        private int currentFrame;
        private Color color;
        private Rectangle sourceRect = new Rectangle();
        private Rectangle destinationRect = new Rectangle();

        private int frameWidth;
        private int frameHeight;
        private bool isActive;
        private bool looping;
        private Vector2 position;

        /// <summary>
        /// Gets or sets the width of the frame.
        /// </summary>
        /// <value>
        /// The width of the frame.
        /// </value>
        public int FrameWidth
        {
            get
            {
                return frameWidth;
            }
            set
            {
                frameWidth = value;
            }
        }

        /// <summary>
        /// Gets or sets the height of the frame.
        /// </summary>
        /// <value>
        /// The height of the frame.
        /// </value>
        public int FrameHeight
        {
            get
            {
                return frameHeight;
            }
            set
            {
                frameHeight = value;
            }
        }

        /// <summary>
        /// Gets or sets the position.
        /// </summary>
        /// <value>
        /// The position.
        /// </value>
        public Vector2 Position
        {
            get
            {
                return position;
            }
            set
            {
                position = value;
            }
        }

        public bool IsActive
        {
            get
            {
                return isActive;
            }
            set
            {
                isActive = value;
            }
        }

        /// <summary>
        /// Initializes the specified texture.
        /// </summary>
        /// <param name="texture">The texture.</param>
        /// <param name="position">The position.</param>
        /// <param name="frameWidth">Width of the frame.</param>
        /// <param name="color">The color.</param>
        /// <param name="scale">The scale.</param>
        /// <param name="looping">if set to <c>true</c> [looping].</param>
        public void Initialize(
            Texture2D texture,
            Vector2 position,
            int frameWidth,
            Color color,
            float scale,
            bool looping)
        {
            this.color = color;
            this.frameWidth = frameWidth;
            this.frameHeight = texture.Height;
            this.frameCount = texture.Width / frameWidth;
            this.scale = scale;

            this.looping = looping;
            this.position = position;
            spriteStrip = texture;

            currentFrame = 0;

            isActive = true;
        }

        public void Update()
        {
            if (isActive == false)
            {
                return;
            }

            currentFrame++;

            if (currentFrame == frameCount)
            {
                currentFrame = 0;
                if (looping == false)
                {
                    isActive = false;
                }
            }

            sourceRect = new Rectangle(currentFrame * frameWidth, 0, frameWidth, frameHeight);

            destinationRect = new Rectangle(
                (int)position.X - ((int)(frameWidth * scale) / 2),
                (int)position.Y - ((int)(frameHeight * scale) / 2),
                (int)(frameWidth * scale),
                (int)(frameHeight * scale));
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (isActive)
            {
                spriteBatch.Draw(spriteStrip, destinationRect, sourceRect, color);
            }
        }
    }
}