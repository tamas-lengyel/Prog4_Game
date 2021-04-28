// <copyright file="ActiveGameObjects.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Model
{
    using System.Windows;

    /// <summary>
    /// Active gameobject class.
    /// </summary>
    public abstract class ActiveGameObjects : GameObjects
    {
        /// <summary>
        /// Gets or sets the coordinates.
        /// </summary>
        public abstract override Point Cords { get; set; }

        /// <summary>
        /// Gets or sets health.
        /// </summary>
        public abstract int Health { get; set; }

        /// <summary>
        /// Gets or sets the damege.
        /// </summary>
        public abstract int Damage { get; set; }
    }
}