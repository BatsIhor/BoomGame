using System;
using HoneycombRush.ScreenManagerLogic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HoneycombRush.Screens
{
    internal class BackgroundScreen : GameScreen
    {
        private Texture2D background;
        private string backgroundName;

        /// <summary>
        /// Creates a new background screen.
        /// </summary>
        /// <param name="backgroundName">Name of the background texture to use.</param>
        public BackgroundScreen(string backgroundName)
        {
            TransitionOnTime = TimeSpan.FromSeconds(0.0);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);

            this.backgroundName = backgroundName;
        }

        /// <summary>
        /// Load screen resources.
        /// </summary>
        public override void LoadContent()
        {
            background = ScreenManager.Game.Content.Load<Texture2D>("Textures/Backgrounds/" + backgroundName);
        }

        /// <summary>
        /// Update the screen.
        /// </summary>
        /// <param name="gameTime">Game time information.</param>
        /// <param name="otherScreenHasFocus">Whether or not another screen currently has the focus.</param>
        /// <param name="coveredByOtherScreen">Whether or not this screen is covered by another.</param>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, false);
        }

        /// <summary>
        /// Renders the screen.
        /// </summary>
        /// <param name="gameTime">Game time information.</param>
        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

            spriteBatch.Begin();

            // Draw background
            spriteBatch.Draw(background, new Vector2(0, 0), Color.White*TransitionAlpha);

            spriteBatch.End();
        }
    }
}