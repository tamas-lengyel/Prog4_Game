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
        public PowerupType Type;
        public int ModifyRate { get; }
        public Powerups()
        {
            switch (Type)
            {
                case PowerupType.Health:
                    ModifyRate = 20;
                    break;
                case PowerupType.Damage:
                    ModifyRate = 5;
                    break;
                case PowerupType.FiringSpeed:
                    ModifyRate = 2;
                    break;
            }
        }
    }
}
