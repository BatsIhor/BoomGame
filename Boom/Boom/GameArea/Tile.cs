using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Boom.GameArea
{
    /// <summary>
    /// Controls the collision detection and response behavior of a tile.
    /// </summary>
    public enum TileTypes
    {
        /// <summary>
        /// A passable tile is one which does not hinder player motion at all.
        /// </summary>
        Passable = 0,

        /// <summary>
        /// An impassable tile is one which does not allow the player to move through
        /// it at all. It is completely solid.
        /// </summary>
        Impassable = 1,

        /// <summary>
        /// A platform tile is one which behaves like a passable tile except when the
        /// player is above it. A player can jump up through a platform as well as move
        /// past it to the left and right, but can not fall down through the top of it.
        /// </summary>
        Platform = 2,
    }

    /// <summary>
    /// Stores the appearance and collision behavior of a tile.
    /// </summary>
    public struct Tile
    {
        public const int Width = 40;

        public const int Height = 32;

        private Texture2D texture;
        private TileTypes types;

        public static readonly Vector2 Size = new Vector2(Width, Height);

        /// <summary>
        /// Constructs a new tile.
        /// </summary>
        /// <param name="texture">
        /// The texture.
        /// </param>
        /// <param name="types">
        /// The types.
        /// </param>
        public Tile(Texture2D texture, TileTypes types)
        {
            this.texture = texture;
            this.types = types;
        }

        public TileTypes Collision
        {
            get
            {
                return types;
            }
            set
            {
                types = value;

            }
        }

        public Texture2D Texture
        {
            get
            {
                return texture;
            }
            set
            {
                texture = value;
            }

        }

    }
}
