// <copyright file="BossEnemy.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Model.Active
{
    using System.Collections.Generic;
    using System.Windows;

    /// <summary>
    /// Boss class.
    /// </summary>
    public class BossEnemy : ActiveGameObjects
    {
        /// <summary>
        /// Shooting pattern for the boss's shooting mechanism.
        /// </summary>
        public static readonly List<Point> ShootingPattern = new List<Point>()
        {
            new Point(-1, -1),
            new Point(0, -1),
            new Point(1, -1),
            new Point(1, 0),
            new Point(1, 1),
            new Point(0, 1),
            new Point(-1, 1),
            new Point(-1, 0),
        };

        /// <summary>
        /// Whether the player is an area around the boss.
        /// </summary>
        public bool PlayerInSight;

        /// <summary>
        /// Gets or sets the coordinates.
        /// </summary>
        public override Point Cords { get; set; }

        /// <summary>
        /// Gets or sets health.
        /// </summary>
        public override int Health { get; set; }

        /// <summary>
        /// Gets or sets the damege.
        /// </summary>
        public override int Damage { get; set; }

        /// <summary>
        /// Gets the Area.
        /// </summary>
        public override Rect Area
        {
            get
            {
                return new Rect((this.Cords.X * GameModel.TileSize) - ((GameModel.TileSize * 3) / 2), (this.Cords.Y * GameModel.TileSize) - ((GameModel.TileSize * 3) / 2), (GameModel.TileSize * 3) - 1, (GameModel.TileSize * 3) - 1);
            }
        }
    }
}