using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Model.Passive
{
    public enum PowerupType
    {
        Health, Damage, FiringSpeed
    }

    public class Powerups : PassiveGameObjects
    {
        public override Point Cords { get; set; }
        public PowerupType Type { get; set; }
        public double ModifyRate { get; }
        public Powerups(Point Cords, PowerupType Type)
        {
            this.Cords = Cords;
            this.Type = Type;
            switch (this.Type)
            {
                case PowerupType.Health:
                    ModifyRate = 20;
                    break;
                case PowerupType.Damage:
                    ModifyRate = 5;
                    break;
                case PowerupType.FiringSpeed:
                    ModifyRate = 0.2d;
                    break;
            }
        }
        public override Rect Area { get { return new Rect(Cords.X * GameModel.TileSize, Cords.Y * GameModel.TileSize, GameModel.TileSize - 1, GameModel.TileSize - 1); } }

    }
}
