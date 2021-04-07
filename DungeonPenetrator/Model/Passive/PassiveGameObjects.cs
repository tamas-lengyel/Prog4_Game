using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Model
{
    public abstract class PassiveGameObjects : GameObjects
    {
        public abstract override Point Cords { get; set; }
    }
}
