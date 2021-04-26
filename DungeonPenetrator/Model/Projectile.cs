using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace Model
{
    public enum ProjectileType
    {
        Enemy,Player,Boss
    };
    public class Projectile : GameObjects
    {
        public int Speed { get; set; }
        public override Point Cords { get; set; }
        public Point direction;
        public int Damage { get; set; }
        public Projectile(Point From, Point To)
        {
            this.Cords = From;

            //this.direction = new Point(To.X-From.X, To.Y-From.Y );
            double x = To.X - From.X;
            double y = To.Y - From.Y;
            double magnetude = (Math.Sqrt(Math.Pow(x, 2) + Math.Pow(y, 2)));
            this.direction = new Point( x/magnetude, y / magnetude);

        }
        public ProjectileType Type { get; set; }
        public override Rect Area { get
            {
                switch (Type)
                {
                    case ProjectileType.Enemy or ProjectileType.Player:
                        return new Rect(Cords.X, Cords.Y, 10, 10);
                    case ProjectileType.Boss:
                        return new Rect(Cords.X, Cords.Y, 20, 20);
                    default:
                        return new Rect(Cords.X, Cords.Y, 10, 10);
                }
            } }
        public DispatcherTimer Timer { get; set; }
        /*public override bool Equals(object obj)
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
        }*/
    }
}
