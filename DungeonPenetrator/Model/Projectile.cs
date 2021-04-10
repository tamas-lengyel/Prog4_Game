using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Model
{
    public class Projectile
    {
        public int Speed { get; set; }
        public Point direction;

        public Projectile(Point From, Point To)
        {
            this.direction = new Point(From.X - To.X, From.Y - To.Y);
        }
    }
}
