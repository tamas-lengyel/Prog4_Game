using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Model
{
    public class ShootingEnemy : ActiveGameObjects
    {
        public override Point Cords { get; set; }
        public override int Health { get; set; }
        public override int Damage { get; set; }

        public EventHandler ShootPlayerEvent;
    }
}
