// <copyright file="IGameModel.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

[assembly: System.CLSCompliant(false)]

namespace Model
{
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Threading;
    using Model.Active;
    using Model.Passive;

    /// <summary>
    /// Interface for the GameModel.
    /// </summary>
    public interface IGameModel
    {
        /// <summary>
        /// Gets the GameWidth.
        /// </summary>
        double GameWidth { get; }

        /// <summary>
        /// Gets the GameHeight.
        /// </summary>
        double GameHeight { get; }

        /// <summary>
        /// Gets the TileSize.
        /// </summary>
        static double TileSize { get; }

        /// <summary>
        /// Gets or sets the mousePosition.
        /// </summary>
        Point MousePosition { get; set; }

        /// <summary>
        /// Gets the LevelExit.
        /// </summary>
        Point LevelExit { get; }

        /// <summary>
        /// Gets or sets the LevelCounter.
        /// </summary>
        int LevelCounter { get; set; }

        /// <summary>
        /// Gets or sets the Boss.
        /// </summary>
        BossEnemy Boss { get; set; }

        /// <summary>
        /// Gets or sets the Player.
        /// </summary>
        Player MyPlayer { get; set; }

        /// <summary>
        /// Gets or sets the Projectiles.
        /// </summary>
        List<Projectile> Projectiles { get; set; }

        /// <summary>
        /// Gets or sets the Flying Monsters.
        /// </summary>
        List<FlyingEnemy> FlyingMonsters { get; set; }

        /// <summary>
        /// Gets or sets the Shooting Monsters.
        /// </summary>
        List<ShootingEnemy> ShootingMonsters { get; set; }

        /// <summary>
        /// Gets or sets the Tracking Monsters.
        /// </summary>
        List<TrackingEnemy> TrackingMonsters { get; set; }

        /// <summary>
        /// Gets or sets the Lavas.
        /// </summary>
        List<LavaProp> Lavas { get; set; }

        /// <summary>
        /// Gets or sets the Waters.
        /// </summary>
        List<WaterProp> Waters { get; set; }

        /// <summary>
        /// Gets or sets the Rocks.
        /// </summary>
        List<WallProp> Walls { get; set; }

        /// <summary>
        /// Gets or sets the Powerups.
        /// </summary>
        List<Powerups> Powerups { get; set; }

        /// <summary>
        /// Gets or sets the Game Area.
        /// </summary>
        char[,] GameAreaChar { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the level is finished.
        /// </summary>
        bool LevelFinished { get; set; }

        /// <summary>
        /// Gets or sets the Tracking Path.
        /// </summary>
        Dictionary<Point, Point> BasicTrackingPath { get; set; } // Key tilecord-> Value->DirectionVector

        /// <summary>
        /// Gets or sets the Flying Monster Tracking Path.
        /// </summary>
        Dictionary<Point, Point> FlyingTrackingPath { get; set; } // Key tilecord-> Value->DirectionVector

        /// <summary>
        /// Gets or sets a value indicating whether the game is paused.
        /// </summary>
        bool GameIsPaused { get; set; }

        /// <summary>
        /// Gets or sets the Lava tick timer.
        /// </summary>
        DispatcherTimer LavaTickTimer { get; set; }

        /// <summary>
        /// Gets or sets the Enemy Hit tick timer.
        /// </summary>
        DispatcherTimer EnemyHitTickTimer { get; set; }
    }
}