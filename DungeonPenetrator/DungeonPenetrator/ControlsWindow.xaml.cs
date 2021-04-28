// <copyright file="ControlsWindow.xaml.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace DungeonPenetrator
{
    using System.Windows;

    /// <summary>
    /// Interaction logic for ControlsWindow.xaml.
    /// </summary>
    public partial class ControlsWindow : Window
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ControlsWindow"/> class.
        /// </summary>
        public ControlsWindow()
        {
            this.InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}