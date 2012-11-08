using System.Collections.Generic;
using System.Linq;
using HoneycombRush.Objects;
using HoneycombRush.ScreenManagerLogic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;

namespace HoneycombRush.Logic
{
    internal class ThumbStickLogic
    {
        private Block[,] blocks;
        private Texture2D controlstick;
        private Texture2D controlstickBoundary;
        private Vector2 controlstickBoundaryPosition;
        private Vector2 controlstickStartupPosition;
        private bool isSmokeButtonClicked;
        private Vector2 lastTouchPosition;
        private ScreenManager screenManager;

        private Texture2D smokeButton;
        private Vector2 smokeButtonPosition = new Vector2(664, 346);

        public ThumbStickLogic(ScreenManager screenManager)
        {
            this.screenManager = screenManager;
            controlstickBoundaryPosition = new Vector2(34, 347);
            controlstickStartupPosition = new Vector2(55, 369);
        }

        public Rectangle GetThumbStickArea()
        {
            return new Rectangle((int) controlstickBoundaryPosition.X, (int) controlstickBoundaryPosition.Y,
                                 controlstickBoundary.Width, controlstickBoundary.Height);
        }

        /// <summary>
        /// Draws the bomb button.
        /// </summary>
        internal void Draw(ScreenManager screenManager)
        {
            if (isSmokeButtonClicked)
            {
                screenManager.SpriteBatch.Draw(
                    smokeButton, new Rectangle((int) smokeButtonPosition.X, (int) smokeButtonPosition.Y, 109, 109),
                    new Rectangle(109, 0, 109, 109), Color.White);
            }
            else
            {
                screenManager.SpriteBatch.Draw(
                    smokeButton, new Rectangle((int) smokeButtonPosition.X, (int) smokeButtonPosition.Y, 109, 109),
                    new Rectangle(0, 0, 109, 109), Color.White);
            }

            screenManager.SpriteBatch.Draw(controlstickBoundary, controlstickBoundaryPosition, Color.White);
            screenManager.SpriteBatch.Draw(controlstick, controlstickStartupPosition, Color.White);
        }

        internal void LoadTextures(ScreenManager ScreenManager)
        {
            controlstickBoundary = screenManager.Game.Content.Load<Texture2D>("Textures/controlstickBoundary");
            controlstick = screenManager.Game.Content.Load<Texture2D>("Textures/controlstick");
            smokeButton = ScreenManager.Game.Content.Load<Texture2D>("Textures/smokeBtn");
        }

        /// <summary>
        /// Handle thumbstick logic
        /// </summary>
        public void HandleThumbStick(Block[,] blocks, Bomberman bomberman, InputState input)
        {
            this.blocks = blocks;

            isSmokeButtonClicked = false;
            IEnumerable<Bomb> bombsToBeDeleted = bomberman.Bombs.Where(bomb => bomb.IsExploded);
            if (bombsToBeDeleted.Count() > 0)
            {
                foreach (Bomb bomb in bombsToBeDeleted)
                {
                    screenManager.Game.Components.Remove(bomb);
                    bomb.Stop();
                }
            }

            // If there was any touch
            if (VirtualThumbsticks.RightThumbstickCenter.HasValue)
            {
                // Button BodyRectangle
                Rectangle buttonRectangle = new Rectangle((int) smokeButtonPosition.X, (int) smokeButtonPosition.Y, 60,
                                                          109);

                // Touch BodyRectangle
                Rectangle touchRectangle = new Rectangle((int) VirtualThumbsticks.RightThumbstickCenter.Value.X,
                                                         (int) VirtualThumbsticks.RightThumbstickCenter.Value.Y, 1, 1);

                // If the touch is in the button
                if (buttonRectangle.Contains(touchRectangle))
                {
                    isSmokeButtonClicked = true;
                }
            }

            if (input.TouchState.Count > 0)
            {
                foreach (TouchLocation touch in input.TouchState)
                {
                    lastTouchPosition = touch.Position;
                }
            }

            // Calculate the rectangle of the outer circle of the thumbstick
            Rectangle outerControlstick = new Rectangle(
                0,
                (int) controlstickBoundaryPosition.Y - 35,
                controlstickBoundary.Width + 60,
                controlstickBoundary.Height + 60);

            //handle bomb Button
            if (isSmokeButtonClicked)
            {
                dropBomb(bomberman);
            }

            // Reset the thumbstick position when it is idle
            if (VirtualThumbsticks.LeftThumbstick == Vector2.Zero)
            {
                bomberman.SetMovement(Vector2.Zero);
                controlstickStartupPosition = new Vector2(55, 369);
            }
            else
            {
                // If not in motion and the touch point is not in the control bounds - there is no movement
                Rectangle touchRectangle = new Rectangle((int) lastTouchPosition.X, (int) lastTouchPosition.Y, 1, 1);

                if (!outerControlstick.Contains(touchRectangle))
                {
                    controlstickStartupPosition = new Vector2(55, 369);
                    return;
                }

                // Move the beekeeper
                setMotion(bomberman);

                // Moves the thumbstick's inner circle
                float radious = controlstick.Width/2 + 35;
                controlstickStartupPosition = new Vector2(55, 369) + (VirtualThumbsticks.LeftThumbstick*radious);
            }
        }

        /// <summary>
        /// Drops the bomb.
        /// </summary>
        private void dropBomb(Bomberman bomberman)
        {
            if (bomberman.Bombs.Count > 0)
            {
                Bomb bomb = bomberman.Bombs.First();
                bomb.SetPosition(new Vector2(bomberman.CollisionArea.X, bomberman.CollisionArea.Y - 10));
                bomb.Start();
                bomb.AnimationDefinitions = XmlLogic.LoadAnimationFromXml(screenManager);
                if (!screenManager.Game.Components.Contains(bomb))
                {
                    screenManager.Game.Components.Add(bomb);
                    //bomberman.Bombs.Remove(bomb);
                }
            }
        }

        /// <summary>
        /// Moves the beekeeper.
        /// </summary>
        private void setMotion(Bomberman bomberman)
        {
            Vector2 leftThumbstick = VirtualThumbsticks.LeftThumbstick;

            Vector2 bomberCalculatedPosition = new Vector2(bomberman.CollisionArea.X, bomberman.CollisionArea.Y) +
                                               leftThumbstick*12f;

            if (bomberCalculatedPosition.X < 20 ||
                bomberCalculatedPosition.X + bomberman.CollisionArea.Width > screenManager.GraphicsDevice.Viewport.Width)
            {
                leftThumbstick.X = 0;
            }
            if (bomberCalculatedPosition.Y < 20 ||
                bomberCalculatedPosition.Y + bomberman.CollisionArea.Height >
                screenManager.GraphicsDevice.Viewport.Height)
            {
                leftThumbstick.Y = 0;
            }

            if (leftThumbstick != Vector2.Zero)
            {
                if (bomberman.GetDirection == Bomberman.WalkingDirection.Down ||
                    bomberman.GetDirection == Bomberman.WalkingDirection.Up)
                {
                    if (checkBlockCollision(bomberCalculatedPosition, bomberman))
                    {
                        leftThumbstick.Y = 0;
                        bomberCalculatedPosition = new Vector2(bomberman.CollisionArea.X, bomberman.CollisionArea.Y) +
                                                   leftThumbstick*12;

                        if (!checkBlockCollision(bomberCalculatedPosition, bomberman))
                        {
                            bomberman.SetMovement(leftThumbstick*12f);
                        }
                    }
                }
                else
                {
                    if (checkBlockCollision(bomberCalculatedPosition, bomberman))
                    {
                        leftThumbstick.X = 0;
                        bomberCalculatedPosition = new Vector2(bomberman.CollisionArea.X, bomberman.CollisionArea.Y) +
                                                   leftThumbstick*12;

                        if (!checkBlockCollision(bomberCalculatedPosition, bomberman))
                        {
                            bomberman.SetMovement(leftThumbstick*12f);
                        }
                    }
                }

                bomberCalculatedPosition = new Vector2(bomberman.CollisionArea.X, bomberman.CollisionArea.Y) +
                                           leftThumbstick*12;

                if (!checkBlockCollision(bomberCalculatedPosition, bomberman))
                {
                    bomberman.SetMovement(leftThumbstick*12f);
                }
            }
        }

        /// <summary>
        /// Checks if the beekeeper collides with a Block.
        /// </summary>
        /// <param name="bomberCalculatedPosition">The beekeeper's position.</param>
        /// <returns>True if the beekeeper collides with a Block and false otherwise.</returns>
        private bool checkBlockCollision(Vector2 bomberCalculatedPosition, Bomberman bomberman)
        {
            // We do not use the beekeeper's collision area property as he has not actually moved at this point and
            // is still in his previous position
            Rectangle bomberTempCollisionArea = new Rectangle(
                (int) bomberCalculatedPosition.X,
                (int) bomberCalculatedPosition.Y,
                bomberman.CollisionArea.Width,
                bomberman.CollisionArea.Height);

            foreach (Block block in blocks)
            {
                if (block != null && bomberTempCollisionArea.Intersects(block.CollisionArea))
                {
                    // TODO move bomber out of collision area.
                    return true;
                }
            }
            return false;
        }
    }
}