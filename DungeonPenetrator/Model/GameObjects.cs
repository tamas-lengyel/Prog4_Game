using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Model
{
    public abstract class GameObjects : IGameObjects
    {
        public abstract Point Cords { get; set; }
        public abstract Rect Area { get;}
        public bool IsCollision(GameObjects other)
        {
            return this.Area.IntersectsWith(other.Area);
        }
    }
}
