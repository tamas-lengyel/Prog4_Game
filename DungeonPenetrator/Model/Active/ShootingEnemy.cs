// <copyright file="ShootingEnemy.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Model
{
    using System.Windows;

    /// <summary>
    /// Shooting enemy class.
    /// </summary>
    public class ShootingEnemy : ActiveGameObjects
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
        /// Gets the Area.
        /// </summary>
        public override Rect Area
        {
            get { return new Rect(this.Cords.X * GameModel.TileSize, this.Cords.Y * GameModel.TileSize, GameModel.TileSize - 1, GameModel.TileSize - 1); }
        }
    }
}