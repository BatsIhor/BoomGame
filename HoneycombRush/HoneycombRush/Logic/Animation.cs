using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HoneycombRush.Logic
{
    /// <summary>
    /// Supports animation playback.
    /// </summary>
    public class Animation
    {
        private Texture2D animatedCharacter;
        private Point currentFrame;
        private bool drawWasAlreadyCalledOnce;
        private Point frameSize;
        private int lastSubFrame = -1;

        private TimeSpan lastestChangeTime;
        private Point sheetSize;
        private TimeSpan timeInterval = TimeSpan.Zero;

        /// <summary>
        /// Creates a new instance of the animation class
        /// </summary>
        /// <param name="frameSheet">Texture which is a sheet containing 
        /// the animation frames.</param>
        /// <param name="size">The size of a single frame.</param>
        /// <param name="frameSheetSize">The size of the entire animation sheet.</param>
        public Animation(Texture2D frameSheet, Point size, Point frameSheetSize)
        {
            animatedCharacter = frameSheet;
            frameSize = size;
            sheetSize = frameSheetSize;
            Offset = Vector2.Zero;
        }

        public int FrameCount
        {
            get { return sheetSize.X*sheetSize.Y; }
        }

        public Vector2 Offset { get; set; }

        public int FrameIndex
        {
            get { return sheetSize.X*currentFrame.Y + currentFrame.X; }
            set
            {
                if (value >= sheetSize.X*sheetSize.Y + 1)
                {
                    throw new InvalidOperationException("Specified frame index exceeds available frames");
                }

                currentFrame.Y = value/sheetSize.X;
                currentFrame.X = value%sheetSize.X;
            }
        }

        public bool IsActive { get; private set; }

        /// <summary>
        /// Updates the animation's progress.
        /// </summary>
        /// <param name="gameTime">Game time information.</param>
        /// <param name="isInMotion">Whether or not the animation element itself is
        /// currently in motion.</param>
        public void Update(GameTime gameTime, bool isInMotion)
        {
            Update(gameTime, isInMotion, false);
        }

        /// <summary>
        /// Updates the animation's progress.
        /// </summary>
        /// <param name="gameTime">Game time information.</param>
        /// <param name="isInMotion">Whether or not the animation element itself is
        /// currently in motion.</param>
        /// <param name="runSubAnimation">Sub animation</param>
        public void Update(GameTime gameTime, bool isInMotion, bool runSubAnimation)
        {
            if (IsActive && gameTime.TotalGameTime != lastestChangeTime)
            {
                // See if a time interval between frames is defined
                if (timeInterval != TimeSpan.Zero)
                {
                    // Do nothing until an interval passes
                    if (lastestChangeTime + timeInterval > gameTime.TotalGameTime)
                    {
                        return;
                    }
                }

                lastestChangeTime = gameTime.TotalGameTime;
                if (FrameIndex >= FrameCount)
                {
                    FrameIndex = 0; // Reset the animation
                }
                else
                {
                    // Only advance the animation if the animation element is moving
                    if (isInMotion)
                    {
                        // Do not advance frames before the first draw operation
                        if (drawWasAlreadyCalledOnce)
                        {
                            currentFrame.X++;
                            if (currentFrame.X >= sheetSize.X)
                            {
                                currentFrame.X = 0;
                                currentFrame.Y++;
                            }
                            if (currentFrame.Y >= sheetSize.Y)
                            {
                                currentFrame.Y = 0;
                            }
                            if (lastSubFrame != -1)
                            {
                                lastSubFrame = -1;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Render the animation.
        /// </summary>
        /// <param name="spriteBatch">
        /// SpriteBatch with which the current 
        /// frame will be rendered.
        /// </param>
        /// <param name="position">
        /// The position to draw the current frame.
        /// </param>
        /// <param name="spriteEffect">
        /// SpriteEffect to apply to the 
        /// current frame.
        /// </param>
        /// <param name="origin">
        /// The origin.
        /// </param>
        /// <param name="collisionArea">
        /// The collision Area.
        /// </param>
        /// Offset of the frame.
        public void Draw(SpriteBatch spriteBatch, Vector2 position, SpriteEffects spriteEffect, Vector2 origin,
                         Texture2D collisionArea)
        {
            drawWasAlreadyCalledOnce = true;

            spriteBatch.Draw(
                animatedCharacter,
                position + Offset,
                new Rectangle(frameSize.X*currentFrame.X, frameSize.Y*currentFrame.Y, frameSize.X, frameSize.Y),
                Color.White,
                0f,
                origin,
                1.0f,
                spriteEffect,
                0);

            spriteBatch.Draw(
                collisionArea,
                new Vector2(position.X - 1, position.Y + 39),
                new Rectangle(0, 0, 33, 23),
                Color.White,
                0f,
                Vector2.Zero,
                1.0f,
                spriteEffect,
                0);
        }

        /// <summary>
        /// Render the animation.
        /// </summary>
        /// <param name="spriteBatch">
        /// SpriteBatch with which the current 
        /// frame will be rendered.
        /// </param>
        /// <param name="position">
        /// The position to draw the current frame.
        /// </param>
        /// <param name="spriteEffect">
        /// SpriteEffect to apply to the 
        /// current frame.
        /// </param>
        /// Offset of the frame.
        public void Draw(SpriteBatch spriteBatch, Vector2 position, SpriteEffects spriteEffect)
        {
            drawWasAlreadyCalledOnce = true;

            spriteBatch.Draw(
                animatedCharacter,
                position + Offset,
                new Rectangle(frameSize.X*currentFrame.X, frameSize.Y*currentFrame.Y, frameSize.X, frameSize.Y),
                Color.White,
                0f,
                Vector2.Zero,
                1.0f,
                spriteEffect,
                0);
        }

        /// <summary>
        /// Causes the animation to start playing from a specified frame index.
        /// </summary>
        /// <param name="frameIndex">Frame index to play the animation from.</param>
        public void PlayFromFrameIndex(int frameIndex)
        {
            FrameIndex = frameIndex;
            IsActive = true;
            drawWasAlreadyCalledOnce = false;
        }

        /// <summary>
        /// Sets the range of frames which serves as the sub animation.
        /// </summary>
        /// <param name="startFrame">Start frame for the sub-animation.</param>
        /// <param name="endFrame">End frame for the sub-animation.</param>
        public void SetSubAnimation(int startFrame, int endFrame)
        {
            //this.startFrame = startFrame;
            //this.endFrame = endFrame;
        }

        /// <summary>
        /// Used to set the interval between frames.
        /// </summary>
        /// <param name="interval">The interval between frames.</param>
        public void SetFrameInterval(TimeSpan interval)
        {
            timeInterval = interval;
        }
    }
}