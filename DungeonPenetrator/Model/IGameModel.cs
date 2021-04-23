using Model.Active;
using Model.Passive;
using Model.Ui;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Threading;

namespace Model
{
    public interface IGameModel
    {
        double GameWidth { get; }
        double GameHeight { get; }
        static double TileSize { get; }
        Point mousePosition { get; set; }

        Point LevelExit { get; }
        int LevelCounter { get; set; }

        BossEnemy Boss { get; set; }
        Player MyPlayer { get; set; }
        List<Projectile> Projectiles { get; set; }
        List<FlyingEnemy> FlyingMonsters { get; set; }
        List<ShootingEnemy> ShootingMonsters { get; set; }
        List<TrackingEnemy> TrackingMonsters { get; set; }
        List<LavaProp> Lavas { get; set; }
        List<WaterProp> Waters { get; set; }
        List<WallProp> Walls { get; set; }
        List<Powerups> Powerups { get; set; }
        char[,] GameAreaChar { get; set; }
        bool LevelFinished { get; set; }
        Dictionary<Point, Point> BasicTrackingPath { get; set; } // Key tilecord-> Value->DirectionVector
        Dictionary<Point, Point> FlyingTrackingPath { get; set; } // Key tilecord-> Value->DirectionVector

        bool GameIsPaused { get; set; }
        DispatcherTimer LavaTickTimer { get; set; }
        DispatcherTimer EnemyHitTickTimer { get; set; }
    }
}
