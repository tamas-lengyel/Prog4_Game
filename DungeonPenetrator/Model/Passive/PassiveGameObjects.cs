// <copyright file="PassiveGameObjects.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Model
{
    using System.Windows;

    /// <summary>
    /// Passive GameObjects class.
    /// </summary>
    public abstract class PassiveGameObjects : GameObjects
    {
        /// <summary>
        /// Gets or sets the coordinates.
        /// </summary>
        public abstract override Point Cords { get; set; }
    }
}