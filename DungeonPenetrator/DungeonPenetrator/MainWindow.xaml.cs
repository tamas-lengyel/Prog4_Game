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
        public bool AutoOrManual { get; set; }
        public string LoadFilePath { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow(bool autoOrManual,string loadFilePath)
        {
            this.AutoOrManual = autoOrManual;
            this.LoadFilePath = loadFilePath;

            this.InitializeComponent();

            this.exitButton.Visibility = Visibility.Hidden;

            this.KeyDown += this.MainWindow_KeyDown;
        }

        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                this.exitButton.Visibility = Visibility.Visible;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Are you sure?", "Exit", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                MainMenu menu = new MainMenu();
                menu.Show();
                this.Close();
            }
        }

        private void SaveGameClick(object sender, RoutedEventArgs e)
        {
            
        }
    }
}