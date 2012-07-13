using System;
using System.Collections.Generic;
using System.IO;

using Boom.Logic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Boom.GameArea
{
    public class Level : IDisposable
    {
        // The layer which entities are drawn on top of.
        private const int EntityLayer = 2;
        private static readonly Point InvalidPosition = new Point(-1, -1);

        private Tile[,] tiles;
        private Texture2D[] layers;
        private Player player;
        private ContentManager content;

        // Entities in the level.
        public Player Player
        {
            get { return player; }
        }

        // Key locations in the level.        
        private Point exit = InvalidPosition;

        // Level game state.
        private Random random = new Random(354668);

        // Level content.        
        public ContentManager Content
        {
            get { return content; }
        }

        #region Loading

        /// <summary>
        /// Constructs a new level.
        /// </summary>
        /// <param name="content">
        /// The content.
        /// </param>
        /// <param name="fileStream">
        /// A stream containing the tile data.
        /// </param>
        public Level(ContentManager content, Stream fileStream)
        {
            this.content = content;

            loadTiles(fileStream);

            layers = new Texture2D[3];
            for (int i = 0; i < layers.Length; ++i)
            {
                layers[i] = Content.Load<Texture2D>("Backgrounds/Layer" + i + "_" + 0);
            }
        }

        /// <summary>
        /// Iterates over every tile in the structure file and loads its
        /// appearance and behavior. This method also validates that the
        /// file is well-formed with a player start point, exit, etc.
        /// </summary>
        /// <param name="fileStream">A stream containing the tile data.</param>
        private void loadTiles(Stream fileStream)
        {
            // Load the level and ensure all of the lines are the same length.
            int width;
            List<string> lines = new List<string>();
            using (StreamReader reader = new StreamReader(fileStream))
            {
                string line = reader.ReadLine();
                width = line.Length;
                while (line != null)
                {
                    lines.Add(line);
                    if (line.Length != width)
                    {
                        throw new Exception(string.Format("The length of line {0} is different from all preceeding lines.", lines.Count));
                    }
                    line = reader.ReadLine();
                }
            }

            // Allocate the tile grid.
            tiles = new Tile[width, lines.Count];

            // Loop over every tile position,
            for (int y = 0; y < Height; ++y)
            {
                for (int x = 0; x < Width; ++x)
                {
                    // to load each tile.
                    char tileType = lines[y][x];
                    tiles[x, y] = loadTile(tileType, x, y);
                }
            }

            // Verify that the level has a beginning and an end.
            if (Player == null)
            {
                throw new NotSupportedException("A level must have a starting point.");
            }
            if (exit == InvalidPosition)
            {
                throw new NotSupportedException("A level must have an exit.");
            }
        }

        /// <summary>
        /// Loads an individual tile's appearance and behavior.
        /// </summary>
        /// <param name="tileType">
        /// The character loaded from the structure file which
        /// indicates what should be loaded.
        /// </param>
        /// <param name="x">
        /// The X location of this tile in tile space.
        /// </param>
        /// <param name="y">
        /// The Y location of this tile in tile space.
        /// </param>
        /// <returns>The loaded tile.</returns>
        private Tile loadTile(char tileType, int x, int y)
        {
            switch (tileType)
            {
                // Blank space
                case '.':
                    return new Tile(null, TileTypes.Passable);

                // Exit
                case 'X':
                    return loadExitTile(x, y);

                // Floating platform
                case '-':
                    return loadTile("Platform", TileTypes.Platform);

                // Platform block
                case '~':
                    return loadVarietyTile("BlockB", 2, TileTypes.Platform);

                // Passable block
                case ':':
                    return loadVarietyTile("BlockB", 2, TileTypes.Passable);

                // Player 1 start point
                case '1':
                    return loadStartTile();

                // Impassable block
                case '#':
                    return loadVarietyTile("BlockA", 7, TileTypes.Impassable);

                // Unknown tile type character
                default:
                    throw new NotSupportedException(string.Format("Unsupported tile type character '{0}' at position {1}, {2}.", tileType, x, y));
            }
        }

        /// <summary>
        /// Creates a new tile. The other tile loading methods typically chain to this
        /// method after performing their special logic.
        /// </summary>
        /// <param name="name">
        /// Path to a tile texture relative to the Content/Tiles directory.
        /// </param>
        /// <param name="collision">
        /// The tile collision type for the new tile.
        /// </param>
        /// <returns>The new tile.</returns>
        private Tile loadTile(string name, TileTypes collision)
        {
            return new Tile(Content.Load<Texture2D>("Tiles/" + name), collision);
        }

        /// <summary>
        /// Loads a tile with a random appearance.
        /// </summary>
        /// <param name="baseName">
        /// The content name prefix for this group of tile variations. Tile groups are
        /// name LikeThis0.png and LikeThis1.png and LikeThis2.png.
        /// </param>
        /// <param name="variationCount">
        /// The number of variations in this group.
        /// </param>
        /// <param name="collision">
        /// The collision.
        /// </param>
        /// <returns>
        /// The load variety tile.
        /// </returns>
        private Tile loadVarietyTile(string baseName, int variationCount, TileTypes collision)
        {
            int index = random.Next(variationCount);
            return loadTile(baseName + index, collision);
        }

        /// <summary>
        /// Instantiates a player, puts him in the level, and remembers where to put him when he is resurrected.
        /// </summary>
        /// <returns>
        /// The load start tile.
        /// </returns>
        private Tile loadStartTile()
        {
            if (Player != null)
            {
                throw new NotSupportedException("A level may only have one starting point.");
            }

            player = new Player();

            return new Tile(null, TileTypes.Passable);
        }

        /// <summary>
        /// Remembers the location of the level's exit.
        /// </summary>
        /// <param name="x">
        /// The x coordinates.
        /// </param>
        /// <param name="y">
        /// The y coordinates.
        /// </param>
        /// <returns>
        /// The load exit tile.
        /// </returns>
        private Tile loadExitTile(int x, int y)
        {
            if (exit != InvalidPosition)
            {
                throw new NotSupportedException("A level may only have one exit.");
            }

            exit = GetBounds(x, y).Center;

            return loadTile("Exit", TileTypes.Passable);
        }

        /// <summary>
        /// Unloads the level content.
        /// </summary>
        public void Dispose()
        {
            Content.Unload();
        }

        #endregion

        #region Bounds and collision

        /// <summary>
        /// Gets the bounding rectangle of a tile in world space.
        /// </summary>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        /// <returns>Rectangle by coordinates</returns>
        public Rectangle GetBounds(int x, int y)
        {
            return new Rectangle(x * Tile.Width, y * Tile.Height, Tile.Width, Tile.Height);
        }

        /// <summary>
        /// Width of level measured in tiles.
        /// </summary>
        public int Width
        {
            get { return tiles.GetLength(0); }
        }

        /// <summary>
        /// Height of the level measured in tiles.
        /// </summary>
        public int Height
        {
            get { return tiles.GetLength(1); }
        }

        #endregion

        #region Update

        /// <summary>
        /// Updates all objects in the world, performs collision between them,
        /// and handles the time limit with scoring.
        /// </summary>
        public void Update()
        {
            if (!Player.IsActive)
            {
            }
            else
            {
            }
        }

        #endregion

        #region Draw

        /// <summary>
        /// Draw everything in the level from background to foreground.
        /// </summary>
        /// <param name="spriteBatch">
        /// The sprite Batch.
        /// </param>
        public void Draw(SpriteBatch spriteBatch)
        {
            for (int i = 0; i <= EntityLayer; ++i)
            {
                spriteBatch.Draw(layers[i], Vector2.Zero, Color.White);
            }

            drawTiles(spriteBatch);

            for (int i = EntityLayer + 1; i < layers.Length; ++i)
            {
                spriteBatch.Draw(layers[i], Vector2.Zero, Color.White);
            }
        }

        /// <summary>
        /// Draws each tile in the level.
        /// </summary>
        /// <param name="spriteBatch">
        /// The sprite Batch.
        /// </param>
        private void drawTiles(SpriteBatch spriteBatch)
        {
            // For each tile position
            for (int y = 0; y < Height; ++y)
            {
                for (int x = 0; x < Width; ++x)
                {
                    // If there is a visible tile in that position
                    Texture2D texture = tiles[x, y].Texture;
                    if (texture != null)
                    {
                        // Draw it in screen space.
                        Vector2 position = new Vector2(x, y) * Tile.Size;
                        spriteBatch.Draw(texture, position, Color.White);
                    }
                }
            }
        }

        #endregion
    }
}
