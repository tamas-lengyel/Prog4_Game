using Model.Active;
using Model.Passive;
using Model.Ui;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Model
{
    public class GameModel : IGameModel
    {
        public double GameWidth => 650;
        public double GameHeight => 800;
        static public double TileSize => 50;
        public Point mousePosition { get; set; }

        public Point LevelExit => new Point((int)(GameWidth/TileSize/ 2),0);
        public int LevelCounter { get; set; }

        public BossEnemy Boss { get; set; }
        public Player MyPlayer { get; set; }
        public List<Projectile> Projectiles { get; set; }
        public List<FlyingEnemy> FlyingMonsters { get; set; }
        public List<ShootingEnemy> ShootingMonsters { get; set; }
        public List<TrackingEnemy> TrackingMonsters { get; set; }
        public List<LavaProp> Lavas { get; set; }
        public List<WaterProp> Waters { get; set; }
        public List<WallProp> Walls { get; set; }
        public List<Powerups> Powerups { get; set; }
        public char[,] GameAreaChar { get; set; }
        public bool LevelFinished{ get; set; }
        public Dictionary<Point,Point> BasicTrackingPath { get; set; } // Key tilecord-> Value->DirectionVector
        public Dictionary<Point, Point> FlyingTrackingPath { get; set; } // Key tilecord-> Value->DirectionVector

        public bool GameIsPaused { get; set; }
    }
}
