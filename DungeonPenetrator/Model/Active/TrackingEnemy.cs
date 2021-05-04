// <copyright file="TrackingEnemy.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Model.Active
{
    using System.Windows;

    /// <summary>
    /// Tracking enemy class.
    /// </summary>
    public class TrackingEnemy : ActiveGameObjects
    {
        /// <summary>
        /// Gets or sets the coordinates.
        /// </summary>
        public override Point Cords { get; set; }

        /// <summary>
        /// Gets or sets health.
        /// </summary>
        public override int Health { get; set; }

        /// <summary>
        /// Gets or sets the damage.
        /// </summary>
        public override int Damage { get; set; }

        /// <summary>
        /// Gets the area.
        /// </summary>
        public override Rect Area
        {
            get { return new Rect(this.Cords.X * GameModel.TileSize, this.Cords.Y * GameModel.TileSize, GameModel.TileSize - 1, GameModel.TileSize - 1); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the enemy can attack.
        /// </summary>
        public bool CanAttack { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the enemy is being damaged by lava.
        /// </summary>
        public bool BeingDamagedByLava { get; set; }
    }
}