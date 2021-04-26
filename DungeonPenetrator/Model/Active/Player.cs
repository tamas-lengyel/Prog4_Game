using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Model.Active
{
    public class Player : ActiveGameObjects
    {
        public override Point Cords { get; set; }
        public override int Health { get; set; }
        public override int Damage { get; set; }
        public double FiringSpeed { get; set; }
        public bool IsReloading { get; set; }
        public bool BeingDamagedByLava { get; set; }

        public override Rect Area { get { return new Rect(Cords.X * GameModel.TileSize, Cords.Y * GameModel.TileSize, GameModel.TileSize - 1, GameModel.TileSize - 1); } }
    }
}
