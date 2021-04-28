// <copyright file="GameModel.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Model
{
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Threading;
    using Model.Active;
    using Model.Passive;

    /// <summary>
    /// GameModel class.
    /// </summary>
    public class GameModel : IGameModel
    {
        /// <summary>
        /// Gets the tile size.
        /// </summary>
        public static double TileSize => 50;

        /// <inheritdoc/>
        public double GameWidth => 650;

        /// <inheritdoc/>
        public double GameHeight => 800;

        /// <inheritdoc/>
        public Point MousePosition { get; set; }

        /// <inheritdoc/>
        public Point LevelExit => new Point((int)(this.GameWidth / TileSize / 2), 0);

        /// <inheritdoc/>
        public int LevelCounter { get; set; }

        /// <inheritdoc/>
        public BossEnemy Boss { get; set; }

        /// <inheritdoc/>
        public Player MyPlayer { get; set; }

        /// <inheritdoc/>
        public List<Projectile> Projectiles { get; set; }

        /// <inheritdoc/>
        public List<FlyingEnemy> FlyingMonsters { get; set; }

        /// <inheritdoc/>
        public List<ShootingEnemy> ShootingMonsters { get; set; }

        /// <inheritdoc/>
        public List<TrackingEnemy> TrackingMonsters { get; set; }

        /// <inheritdoc/>
        public List<LavaProp> Lavas { get; set; }

        /// <inheritdoc/>
        public List<WaterProp> Waters { get; set; }

        /// <inheritdoc/>
        public List<WallProp> Walls { get; set; }

        /// <inheritdoc/>
        public List<Powerups> Powerups { get; set; }

        /// <inheritdoc/>
        public char[,] GameAreaChar { get; set; }

        /// <inheritdoc/>
        public bool LevelFinished { get; set; }

        /// <inheritdoc/>
        public Dictionary<Point, Point> BasicTrackingPath { get; set; } // Key tilecord-> Value->DirectionVector

        /// <inheritdoc/>
        public Dictionary<Point, Point> FlyingTrackingPath { get; set; } // Key tilecord-> Value->DirectionVector

        /// <inheritdoc/>
        public bool GameIsPaused { get; set; }

        /// <inheritdoc/>
        public DispatcherTimer LavaTickTimer { get; set; }

        /// <inheritdoc/>
        public DispatcherTimer EnemyHitTickTimer { get; set; }
    }
}