using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using HoneycombRush.Objects;
using HoneycombRush.ScreenManagerLogic;

namespace HoneycombRush.Logic
{
    class ThumbStickLogic
    {
        private Vector2 lastTouchPosition;

        private bool isSmokeButtonClicked;
        private bool isInMotion;



        private Bomberman bomberman;

        ScreenManager screenManager;

        private Dictionary<string, Animation> animations;

        //private Block[,] blocks = new Block[15, 11];

        public ThumbStickLogic(ScreenManager screenManager)
        {
            this.screenManager = screenManager;
        }

        /// <summary>
        /// Handle thumbstick logic
        /// </summary>
        private void handleThumbStick(Vector2 controlstickBoundaryPosition, Vector2 controlstickStartupPosition, Texture2D controlstickBoundary, Texture2D controlstick)
        {
            // Calculate the rectangle of the outer circle of the thumbstick
            Rectangle outerControlstick = new Rectangle(
                0,
                (int)controlstickBoundaryPosition.Y - 35,
                controlstickBoundary.Width + 60,
                controlstickBoundary.Height + 60);

            //handle bomb Button
            if (isSmokeButtonClicked)
            {
                dropBomb();
            }

            // Reset the thumbstick position when it is idle
            if (VirtualThumbsticks.LeftThumbstick == Vector2.Zero)
            {
                isInMotion = false;
                bomberman.SetMovement(Vector2.Zero);
                controlstickStartupPosition = new Vector2(55, 369);
            }
            else
            {
                // If not in motion and the touch point is not in the control bounds - there is no movement
                Rectangle touchRectangle = new Rectangle((int)lastTouchPosition.X, (int)lastTouchPosition.Y, 1, 1);

                if (!outerControlstick.Contains(touchRectangle))
                {
                    controlstickStartupPosition = new Vector2(55, 369);
                    isInMotion = false;
                    return;
                }

                // Move the beekeeper
                setMotion();

                // Moves the thumbstick's inner circle
                float radious = controlstick.Width / 2 + 35;
                controlstickStartupPosition = new Vector2(55, 369) + (VirtualThumbsticks.LeftThumbstick * radious);
            }
        }

        /// <summary>
        /// Drops the bomb.
        /// </summary>
        private void dropBomb()
        {
            if (bomberman.Bombs.Count > 0)
            {
                Bomb bomb = bomberman.Bombs.First();
                bomb.SetPosition(new Vector2(bomberman.CollisionArea.X, bomberman.CollisionArea.Y - 10));
                bomb.AnimationDefinitions = animations;
                if (!screenManager.Game.Components.Contains(bomb))
                {
                    screenManager.Game.Components.Add(bomb);
                    bomberman.Bombs.Remove(bomb);
                }
            }
        }

        /// <summary>
        /// Moves the beekeeper.
        /// </summary>
        private void setMotion()
        {
            Vector2 leftThumbstick = VirtualThumbsticks.LeftThumbstick;

            Vector2 bomberCalculatedPosition = new Vector2(bomberman.CollisionArea.X, bomberman.CollisionArea.Y) + leftThumbstick * 12f;

            if (bomberCalculatedPosition.X < 20 || bomberCalculatedPosition.X + bomberman.CollisionArea.Width > screenManager.GraphicsDevice.Viewport.Width)
            {
                leftThumbstick.X = 0;
            }
            if (bomberCalculatedPosition.Y < 20 || bomberCalculatedPosition.Y + bomberman.CollisionArea.Height > screenManager.GraphicsDevice.Viewport.Height)
            {
                leftThumbstick.Y = 0;
            }

            if (leftThumbstick == Vector2.Zero)
            {
                isInMotion = false;
            }
            else
            {
                if (bomberman.GetDirection == Bomberman.WalkingDirection.Down || bomberman.GetDirection == Bomberman.WalkingDirection.Up)
                {
                    if (checkBlockCollision(bomberCalculatedPosition))
                    {
                        leftThumbstick.Y = 0;
                        bomberCalculatedPosition = new Vector2(bomberman.CollisionArea.X, bomberman.CollisionArea.Y) + leftThumbstick * 12;

                        if (!checkBlockCollision(bomberCalculatedPosition))
                        {
                            bomberman.SetMovement(leftThumbstick * 12f);
                            isInMotion = true;
                        }
                    }
                }
                else
                {
                    if (checkBlockCollision(bomberCalculatedPosition))
                    {
                        leftThumbstick.X = 0;
                        bomberCalculatedPosition = new Vector2(bomberman.CollisionArea.X, bomberman.CollisionArea.Y) + leftThumbstick * 12;

                        if (!checkBlockCollision(bomberCalculatedPosition))
                        {
                            bomberman.SetMovement(leftThumbstick * 12f);
                            isInMotion = true;
                        }
                    }
                }

                bomberCalculatedPosition = new Vector2(bomberman.CollisionArea.X, bomberman.CollisionArea.Y) + leftThumbstick * 12;

                if (!checkBlockCollision(bomberCalculatedPosition))
                {
                    bomberman.SetMovement(leftThumbstick * 12f);
                    isInMotion = true;
                }
            }
        }

        /// <summary>
        /// Checks if the beekeeper collides with a Block.
        /// </summary>
        /// <param name="bomberCalculatedPosition">The beekeeper's position.</param>
        /// <returns>True if the beekeeper collides with a Block and false otherwise.</returns>
        private bool checkBlockCollision(Vector2 bomberCalculatedPosition)
        {
            // We do not use the beekeeper's collision area property as he has not actually moved at this point and
            // is still in his previous position
            Rectangle bomberTempCollisionArea = new Rectangle(
                (int)bomberCalculatedPosition.X,
                (int)bomberCalculatedPosition.Y,
                bomberman.CollisionArea.Width,
                bomberman.CollisionArea.Height);

            //foreach (Block block in blocks)
            //{
            //    if (block != null && bomberTempCollisionArea.Intersects(block.CollisionArea))
            //    {
            //        // TODO move bomber out of collision area.
            //        return true;
            //    }
            //}
            return false;
        }
    }
}
