using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Model
{
    public abstract class ActiveGameObjects : GameObjects
    {
        public abstract override Point Cords { get; set; }
        public abstract int Health { get; set; }
        public abstract int Damage { get; set; }

    }
}
