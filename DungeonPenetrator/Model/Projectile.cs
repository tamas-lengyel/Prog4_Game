// <copyright file="Projectile.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Model
{
    using System;
    using System.Windows;
    using System.Windows.Threading;

    /// <summary>
    /// Projectile type.
    /// </summary>
    public enum ProjectileType
    {
        /// <summary>
        /// Enemy type.
        /// </summary>
        Enemy,

        /// <summary>
        /// Player type.
        /// </summary>
        Player,

        /// <summary>
        /// Boss type.
        /// </summary>
        Boss,
    }

    /// <summary>
    /// Projectile class.
    /// </summary>
    public class Projectile : GameObjects
    {
        /// <summary>
        /// Direction Point.
        /// </summary>
        public Point Direction;

        /// <summary>
        /// Initializes a new instance of the <see cref="Projectile"/> class.
        /// </summary>
        /// <param name="from">From Point.</param>
        /// <param name="to">To Point.</param>
        public Projectile(Point from, Point to)
        {
            this.Cords = from;

            // this.direction = new Point(To.X-From.X, To.Y-From.Y );
            double x = to.X - from.X;
            double y = to.Y - from.Y;
            double magnetude = Math.Sqrt(Math.Pow(x, 2) + Math.Pow(y, 2));
            this.Direction = new Point(x / magnetude, y / magnetude);
        }

        /// <summary>
        /// Gets or sets the speed.
        /// </summary>
        public int Speed { get; set; }

        /// <summary>
        /// Gets or sets the Coordinate.
        /// </summary>
        public override Point Cords { get; set; }

        /// <summary>
        /// Gets or sets the damage.
        /// </summary>
        public int Damage { get; set; }

        /// <summary>
        /// Gets or sets the Projectile type.
        /// </summary>
        public ProjectileType Type { get; set; }

        /// <summary>
        /// Gets the Area.
        /// </summary>
        public override Rect Area
        {
            get
            {
                switch (this.Type)
                {
                    case ProjectileType.Enemy or ProjectileType.Player:
                        return new Rect(this.Cords.X, this.Cords.Y, 10, 10);

                    case ProjectileType.Boss:
                        return new Rect(this.Cords.X, this.Cords.Y, 40, 40);

                    default:
                        return new Rect(this.Cords.X, this.Cords.Y, 10, 10);
                }
            }
        }

        /// <summary>
        /// Gets or sets the Timer.
        /// </summary>
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