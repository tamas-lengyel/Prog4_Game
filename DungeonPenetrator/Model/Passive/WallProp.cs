using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Model.Passive
{
    public class WallProp : PassiveGameObjects
    {
        public override Point Cords { get; set; }
        public override Rect Area { get { return new Rect(Cords.X * GameModel.TileSize, Cords.Y * GameModel.TileSize, GameModel.TileSize - 1, GameModel.TileSize - 1); } }
    }
}
