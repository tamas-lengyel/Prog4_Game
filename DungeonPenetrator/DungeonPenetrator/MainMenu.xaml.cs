// <copyright file="MainMenu.xaml.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace DungeonPenetrator
{
    using System;
    using System.Windows;
    using System.Windows.Media;
    using Model;
    using Repository;

    /// <summary>
    /// Interaction logic for MainMenu.xaml.
    /// </summary>
    public partial class MainMenu : Window
    {
        private AutoSaveGameRepository asgRepo = new AutoSaveGameRepository();


        // private MediaPlayer player = new MediaPlayer();

        /// <summary>
        /// Initializes a new instance of the <see cref="MainMenu"/> class.
        /// </summary>
        public MainMenu()
        {
            this.InitializeComponent();

            if (this.asgRepo.GetSaveGame() == null)
            {
                this.newOrContinue.Content = "New Game";
            }
            else
            {
                this.newOrContinue.Content = "Continue Game";
            }

            this.VideoPlayer.Play();
        }

        private void NewGameBtn_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mw = new MainWindow(false, "auto");
            mw.Show();
            this.Close();
        }

        private void HighscoresBtn_Click(object sender, RoutedEventArgs e)
        {
            HighScoreWindow win = new HighScoreWindow();
            if (win.HighScores.Count != 0)
            {
                win.Show();
            }
        }

        private void QuitGameBtn_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Are you sure?", "Exit", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                this.Close();
            }
        }

        private void ControlsBtn_Click(object sender, RoutedEventArgs e)
        {
            ControlsWindow win = new ControlsWindow();
            win.Show();
        }

        private void MediaElement_MediaEnded(object sender, RoutedEventArgs e)
        {
            this.VideoPlayer.Position = TimeSpan.FromSeconds(0);
            this.VideoPlayer.Play();
        }

        private void LoadGameManually(object sender, RoutedEventArgs e)
        {
            LoadGameWindow load = new LoadGameWindow();
            if (load.ShowDialog() == true)
            {
                MainWindow mw = new MainWindow(true, load.FilePath);
                mw.Show();
                this.Close();
            }
        }
    }
}