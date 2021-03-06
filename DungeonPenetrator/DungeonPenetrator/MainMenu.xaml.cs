// <copyright file="MainMenu.xaml.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace DungeonPenetrator
{
    using System;
    using System.Drawing;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using Model;
    using Repository;

    /// <summary>
    /// Interaction logic for MainMenu.xaml.
    /// </summary>
    public partial class MainMenu : Window
    {
        private AutoSaveGameRepository asgRepo = new AutoSaveGameRepository();
        private bool mutedState;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainMenu"/> class.
        /// </summary>
        public MainMenu()
        {
            this.InitializeComponent();

            if (this.asgRepo.GetSaveGame() == null)
            {
                this.newOrContinue.Content = "New Game";
                this.newGame.Visibility = Visibility.Hidden;
            }
            else
            {
                this.newOrContinue.Content = "Continue Game";
                this.newGame.Visibility = Visibility.Visible;
            }

            this.VideoPlayer.Play();
        }

        private void NewOrCountinue_Click(object sender, RoutedEventArgs e)
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
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.DefaultExt = ".json";
            dlg.Filter = "Json File (*.json)|*.json";
            dlg.FileName = "*.json";

            // Show open file dialog box
            bool? result = dlg.ShowDialog();

            // Process open file dialog box results
            if (result == true)
            {
                // Open document
                MainWindow mw = new MainWindow(true, dlg.FileName);
                mw.Show();
                this.Close();
            }
        }

        private void NewGame_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("You are going to delete any progress which is not manually saved.\nAre you sure to continue?", "New Game", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                this.asgRepo.Insert(null);
                MainWindow mw = new MainWindow(false, "auto");
                mw.Show();
                this.Close();
            }
        }

        private void Mute_Click(object sender, RoutedEventArgs e)
        {
            if (this.mutedState == true)
            {
                this.VideoPlayer.IsMuted = false;
                this.mutedState = false;

                ImageSource vmi = new BitmapImage(new Uri(@"/Images/nomute.png", UriKind.Relative));

                this.muteImage.Source = vmi;
            }
            else
            {
                this.VideoPlayer.IsMuted = true;
                this.mutedState = true;

                ImageSource vmi = new BitmapImage(new Uri(@"/Images/mute.png", UriKind.Relative));

                this.muteImage.Source = vmi;
            }
        }
    }
}