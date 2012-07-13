using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Boom.Entities
{
    public class Bomberman
    {
        // Walk textures
        private Texture2D walkEast;
        private Texture2D walkNorth;
        private Texture2D walkSouth;
        private Texture2D walkWest;

        // Pickup
        private Texture2D pickupEast;
        private Texture2D pickupNorth;
        private Texture2D pickupSouth;
        private Texture2D pickupWest;

        // Pickup
        private Texture2D punchEast;
        private Texture2D punchNorth;
        private Texture2D punchSouth;
        private Texture2D punchWest;

        private Texture2D currentTexture;

        private Color color;
        private bool isAlive;
        private BomberState state;
        private ContentManager contentManager;

        public Bomberman(ContentManager contentManager, Color color)
        {
            this.color = color;
            isAlive = true;
        }

        public BomberState State { get; set; }

        /// <summary>
        /// Updates bomber state according to the position and keys.
        /// </summary>
        public void Update()
        {
            switch (State)
            {
                case BomberState.WalkNorth:
                    currentTexture = walkNorth;
                    break;
                case BomberState.WalkEast:
                    currentTexture = walkEast;
                    break;
                case BomberState.WalkSouth:
                    currentTexture = walkSouth;
                    break;
                case BomberState.WalkWest:
                    currentTexture = walkWest;
                    break;

                case BomberState.PickupEast:
                    currentTexture = pickupEast;
                    break;
                case BomberState.PickupNorth:
                    currentTexture = pickupNorth;
                    break;
                case BomberState.PickupSouth:
                    currentTexture = pickupSouth;
                    break;
                case BomberState.PickupWest:
                    currentTexture = pickupWest;
                    break;

                case BomberState.PunchEast:
                    currentTexture = punchEast;
                    break;
                case BomberState.PunchNorth:
                    currentTexture = punchNorth;
                    break;
                case BomberState.PunchSouth:
                    currentTexture = punchSouth;
                    break;
                case BomberState.PunchWest:
                    currentTexture = punchWest;
                    break;
            }
        }

        /// <summary>
        /// Gets the current texture.
        /// </summary>
        public Texture2D CurrentTexture
        {
            get
            {
                return currentTexture;
            }
            private set
            {
                currentTexture = value;
            }
        }
        
        protected void LoadContent()
        {
            walkEast = contentManager.Load<Texture2D>("BombWalkE");
            walkNorth = contentManager.Load<Texture2D>("BombWalkN");
            walkSouth = contentManager.Load<Texture2D>("BombWalkS");
            walkWest = contentManager.Load<Texture2D>("BombWalkW");

            pickupEast = contentManager.Load<Texture2D>("PickupE");
            pickupNorth = contentManager.Load<Texture2D>("PickupN");
            pickupSouth = contentManager.Load<Texture2D>("PickupS");
            pickupWest = contentManager.Load<Texture2D>("PickupW");

            punchEast = contentManager.Load<Texture2D>("PunchE");
            punchNorth = contentManager.Load<Texture2D>("PunchN");
            punchSouth = contentManager.Load<Texture2D>("PunchS");
            punchWest = contentManager.Load<Texture2D>("PunchW");
        }
    }
}