using System;
using HoneycombRush.Logic;
using HoneycombRush.ScreenManagerLogic;
using HoneycombRush.Screens;
using Microsoft.Xna.Framework;

namespace HoneycombRush
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class BomberRush : Game
    {
        private GraphicsDeviceManager graphics;

        private ScreenManager screenManager;

        public BomberRush()
        {
            // Initialize sound system
            AudioManager.Initialize(this);
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            // Frame rate is 30 fps by default for Windows Phone.
            TargetElapsedTime = TimeSpan.FromTicks(333333);


            graphics.IsFullScreen = true;

            // Create a new instance of the Screen Manager
            screenManager = new ScreenManager(this);

            screenManager.AddScreen(new BackgroundScreen("titleScreen"), null);
            screenManager.AddScreen(new MainMenuScreen(), PlayerIndex.One);
            Components.Add(screenManager);
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            HighScoreScreen.LoadHighscores();

            base.LoadContent();
        }
    }
}