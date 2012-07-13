#region File Description
//-----------------------------------------------------------------------------
// WorkerBee.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements


using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


#endregion

namespace HoneycombRush
{
    /// <summary>
    /// A component that represents a worker bee.
    /// </summary>
    public class WorkerBombBase : BombBase
    {
        #region Properties


        protected override int MaxVelocity
        {
            get
            {
                var configuration = ConfigurationManager.ModesConfiguration[ConfigurationManager.DifficultyMode.Value];

                return (int)configuration.MaxWorkerBeeVelocity;
            }
        }

        protected override float AccelerationFactor
        {
            get
            {
                return 1.5f;
            }
        }


        #endregion

        #region Initialization

        /// <summary>
        /// Creates a new worker bee instance.
        /// </summary>
        /// <param name="game">The associated game object.</param>
        /// <param name="gamePlayScreen">The gameplay screen where the bee is displayed</param>
        /// <param name="block">The bee's associated Block.</param>
        public WorkerBombBase(Game game,GameplayScreen gamePlayScreen, Block block)
            : base(game,gamePlayScreen,block)
        {
            AnimationKey = "WorkerBee";
        }

        protected override void LoadContent()
        {
            texture = Game.Content.Load<Texture2D>("Textures/beeWingFlap");

            base.LoadContent();
        }


        #endregion
    }
}
