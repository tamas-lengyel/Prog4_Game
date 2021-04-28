// <copyright file="Powerups.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Model.Passive
{
    using System.Windows;

    /// <summary>
    /// PowerType enum.
    /// </summary>
    public enum PowerupType
    {
        /// <summary>
        /// Health type.
        /// </summary>
        Health,

        /// <summary>
        /// Damage type.
        /// </summary>
        Damage,

        /// <summary>
        /// FiringSpeed type.
        /// </summary>
        FiringSpeed,
    }

    /// <summary>
    /// Powerups class.
    /// </summary>
    public class Powerups : PassiveGameObjects
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Powerups"/> class.
        /// </summary>
        /// <param name="cords">Cords param.</param>
        /// <param name="type">Type param.</param>
        public Powerups(Point cords, PowerupType type)
        {
            this.Cords = cords;
            this.Type = type;
            switch (this.Type)
            {
                case PowerupType.Health:
                    this.ModifyRate = 20;
                    break;

                case PowerupType.Damage:
                    this.ModifyRate = 2;
                    break;

                case PowerupType.FiringSpeed:
                    this.ModifyRate = 0.1d;
                    break;
            }
        }

        /// <summary>
        /// Gets or sets the coordinates.
        /// </summary>
        public override Point Cords { get; set; }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        public PowerupType Type { get; set; }

        /// <summary>
        /// Gets the modify rate.
        /// </summary>
        public double ModifyRate { get; }

        /// <summary>
        /// Gets the Area.
        /// </summary>
        public override Rect Area
        {
            get { return new Rect(this.Cords.X * GameModel.TileSize, this.Cords.Y * GameModel.TileSize, GameModel.TileSize - 1, GameModel.TileSize - 1); }
        }
    }
}