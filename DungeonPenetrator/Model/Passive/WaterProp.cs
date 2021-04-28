// <copyright file="WaterProp.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Model.Passive
{
    using System.Windows;

    /// <summary>
    /// WaterProp class.
    /// </summary>
    public class WaterProp : PassiveGameObjects
    {
        /// <summary>
        /// Gets or sets the coordinate.
        /// </summary>
        public override Point Cords { get; set; }

        /// <summary>
        /// Gets the Area.
        /// </summary>
        public override Rect Area
        {
            get { return new Rect(this.Cords.X * GameModel.TileSize, this.Cords.Y * GameModel.TileSize, GameModel.TileSize - 1, GameModel.TileSize - 1); }
        }
    }
}