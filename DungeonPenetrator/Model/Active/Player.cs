// <copyright file="Player.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Model.Active
{
    using System.Windows;

    /// <summary>
    /// Player class.
    /// </summary>
    public class Player : ActiveGameObjects
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
        /// Gets or sets the firing speed.
        /// </summary>
        public double FiringSpeed { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the player is reloding.
        /// </summary>
        public bool IsReloading { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the player is being damaged by lava.
        /// </summary>
        public bool BeingDamagedByLava { get; set; }

        /// <summary>
        /// Gets the Area.
        /// </summary>
        public override Rect Area
        {
            get { return new Rect(this.Cords.X * GameModel.TileSize, this.Cords.Y * GameModel.TileSize, GameModel.TileSize - 1, GameModel.TileSize - 1); }
        }
    }
}