using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Model
{
    public enum ProjectileType
    {
        Enemy,Player
    };
    public class Projectile
    {
        public int Speed { get; set; }
        public Point Cords { get; set; }
        public Point direction;
        public int Damage { get; set; }
        public Projectile(Point From, Point To)
        {
            this.Cords = From;
            this.direction = new Point(From.X - To.X, From.Y - To.Y);
        }
        public ProjectileType Type { get; set; }

        public override bool Equals(object obj)
        {
            if (obj is Projectile)
            {
                Projectile prj = obj as Projectile;
                return this.Cords == prj.Cords &&
                    this.Damage == prj.Damage &&
                    this.direction == prj.direction &&
                    this.Speed == prj.Speed;

            }
            return false;
        }

        public override int GetHashCode()
        {
            return 0;
        }
    }
}
