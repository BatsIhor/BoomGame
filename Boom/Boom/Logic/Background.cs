using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Boom.Logic
{
    public class Background
    {
        private Texture2D texture;

        /// <summary>
        /// Initializes the specified content.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <param name="texturePath">The texture path.</param>
        public void Initialize(ContentManager content, string texturePath)
        {
            this.texture = content.Load<Texture2D>(texturePath);
        }

        public void Update()
        {
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, new Vector2(0, 0), Color.White);
        }
    }
}