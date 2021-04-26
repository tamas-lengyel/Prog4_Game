using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Model.Active
{
    public class BossEnemy : ActiveGameObjects
    {
        public override Point Cords { get; set; }
        public override int Health { get; set; }
        public override int Damage { get; set; }
        public static List<Point> ShootingPattern = new List<Point>()
            {
            new Point(-1,-1),
            new Point(0,-1),
            new Point(1,-1),
            new Point(1,0),
            new Point(1,1),
            new Point(0,1),
            new Point(-1,1),
            new Point(-1,0),
        };
        public bool PlayerInSight;
        public override Rect Area { get { return new Rect(Cords.X * GameModel.TileSize, Cords.Y * GameModel.TileSize, GameModel.TileSize - 1, GameModel.TileSize - 1); } }
    }
}
