// <copyright file="MainWindow.xaml.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace DungeonPenetrator
{
    using System;
    using System.IO;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Input;
    using Microsoft.Win32;
    using Model;
    using Repository;

    /// <summary>
    /// Interaction logic for MainWindow.xaml.
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        /// <param name="autoOrManual">Auto or Manual saving.</param>
        /// <param name="loadFilePath">File Path.</param>
        public MainWindow(bool autoOrManual, string loadFilePath)
        {
            this.AutoOrManual = autoOrManual;
            this.LoadFilePath = loadFilePath;

            this.InitializeComponent();
        }

        /// <summary>
        /// Gets or sets a value indicating whether the saving is Auto or Manual.
        /// </summary>
        public bool AutoOrManual { get; set; } // Manual = true auto=false

        /// <summary>
        /// Gets or sets the file path.
        /// </summary>
        public string LoadFilePath { get; set; }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Are you sure?", "Exit", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                MainMenu menu = new MainMenu();
                menu.Show();
                this.Close();
            }
        }
    }
}