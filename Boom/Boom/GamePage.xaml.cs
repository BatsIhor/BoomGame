using System;
using System.IO;
using System.Windows;
using System.Windows.Navigation;

using Boom.GameArea;
using Boom.Logic;

using Microsoft.Phone.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;

namespace Boom
{
    /// <summary>
    /// Main game page.
    /// </summary>
    public partial class GamePage : PhoneApplicationPage
    {
        private ContentManager contentManager;
        private GameTimer timer;
        private SpriteBatch spriteBatch;
        private Player player;
        private Level level;
        private Background mainBackground;
        
        // A movement speed for the player
        private float playerMoveSpeed;

        public GamePage()
        {
            InitializeComponent();

            App app = Application.Current as App;
            if (app != null)
            {
                contentManager = app.Content;
            }

            timer = new GameTimer { UpdateInterval = TimeSpan.FromTicks(333333) };
            timer.Update += onUpdate;
            timer.Draw += onDraw;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            SharedGraphicsDeviceManager.Current.GraphicsDevice.SetSharingMode(true);

            spriteBatch = new SpriteBatch(SharedGraphicsDeviceManager.Current.GraphicsDevice);

            player = new Player();
            playerMoveSpeed = 8.0f;

            TouchPanel.EnabledGestures = GestureType.FreeDrag;

            this.loadContent();

            timer.Start();

            base.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            // Stop the timer
            timer.Stop();

            // Set the sharing mode of the graphics device to turn off XNA rendering
            SharedGraphicsDeviceManager.Current.GraphicsDevice.SetSharingMode(false);

            base.OnNavigatedFrom(e);
        }

        private void loadContent()
        {
            // Load the player resources
            Animation playerAnimation = new Animation();
            Texture2D playerTexture = this.contentManager.Load<Texture2D>("BombWalkE");
            playerAnimation.Initialize(playerTexture, Vector2.Zero, 73, Color.White, 1f, true);

            Vector2 playerPosition = new Vector2(
                SharedGraphicsDeviceManager.Current.GraphicsDevice.Viewport.TitleSafeArea.X,
                SharedGraphicsDeviceManager.Current.GraphicsDevice.Viewport.TitleSafeArea.Y
                + SharedGraphicsDeviceManager.Current.GraphicsDevice.Viewport.TitleSafeArea.Height / 2);

            this.player.Initialize(playerAnimation, playerPosition);

            // Load background
            this.mainBackground = new Background();


            // Load Level

            // Load the level.
            string levelPath = string.Format("Levels/0.txt");
            using (Stream fileStream = TitleContainer.OpenStream(levelPath))
            {
                level = new Level(contentManager, fileStream);
            }

            this.mainBackground.Initialize(this.contentManager, "Back");
        }

        private void onUpdate(object sender, GameTimerEventArgs e)
        {
            updatePlayer();
        }

        private void onDraw(object sender, GameTimerEventArgs e)
        {
            SharedGraphicsDeviceManager.Current.GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();
            mainBackground.Draw(spriteBatch);
            level.Draw(spriteBatch);
            player.Draw(spriteBatch);
            spriteBatch.End();
        }

        private void updatePlayer()
        {
            player.Update();

            while (TouchPanel.IsGestureAvailable)
            {
                GestureSample gesture = TouchPanel.ReadGesture();
                if (gesture.GestureType == GestureType.FreeDrag)
                {
                    player.X += gesture.Delta.X;
                    player.Y += gesture.Delta.Y;
                }
            }

            player.X = MathHelper.Clamp(player.X, player.Width / 2, SharedGraphicsDeviceManager.Current.GraphicsDevice.Viewport.Width - (player.Width / 2));
            player.Y = MathHelper.Clamp(player.Y, player.Height / 2, SharedGraphicsDeviceManager.Current.GraphicsDevice.Viewport.Height - (player.Height / 2));
        }
    }
}