// <copyright file="GameObjects.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Model
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows;

    /// <summary>
    /// GameObject class.
    /// </summary>
    public abstract class GameObjects : IGameObjects
    {
        /// <inheritdoc/>
        public abstract Point Cords { get; set; }

        /// <summary>
        /// Gets the Area.
        /// </summary>
        public abstract Rect Area { get; }

        /// <summary>
        /// Intersect examination.
        /// </summary>
        /// <param name="other">Other game object.</param>
        /// <returns>Whether two objects collided.</returns>
        public bool IsCollision(GameObjects other)
        {
            return this.Area.IntersectsWith(other.Area);
        }
    }
}
