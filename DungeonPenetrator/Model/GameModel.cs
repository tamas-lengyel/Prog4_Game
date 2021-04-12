using Model.Active;
using Model.Passive;
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
        public double GameWidth => 700;
        public double GameHeight => 1000;
        public double TileSize => 100;
        public Point LevelExit => new Point((int)(GameWidth / 2), 0);
        public int LevelCounter { get; set; }
        public BossEnemy Boss { get; set; }
        public Player MyPlayer { get; set; }
        public List<FlyingEnemy> FlyingMonster { get; set; }
        public List<ShootingEnemy> ShootingMonster { get; set; }
        public List<TrackingEnemy> TrackingMonster { get; set; }
        public List<LavaProp> Lava { get; set; }
        public List<WaterProp> Water { get; set; }
        public List<WallProp> Wall { get; set; }
        public List<Powerups> Powerup { get; set; }
        public char[,] GameAreaChar { get; set; }
        public bool LevelFinished{ get; set; }
    }
}
