// <copyright file="MainMenu.xaml.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace DungeonPenetrator
{
    using System.Windows;
    using System.Windows.Media;
    using Repository;

    /// <summary>
    /// Interaction logic for MainMenu.xaml.
    /// </summary>
    public partial class MainMenu : Window
    {
        private SaveGameRepository sgRepo = new SaveGameRepository();

        // private MediaPlayer player = new MediaPlayer();

        /// <summary>
        /// Initializes a new instance of the <see cref="MainMenu"/> class.
        /// </summary>
        public MainMenu()
        {
            this.InitializeComponent();

            if (this.sgRepo.GetSaveGame() == null)
            {
                this.newOrContinue.Content = "New Game";
            }
            else
            {
                this.newOrContinue.Content = "Continue Game";
            }

            // player.Open(new Uri("pack://application:,,,/Images/rugoskes.mp4", UriKind.Absolute));
            // VideoDrawing drawing = new VideoDrawing { Rect = new Rect(0, 0, 800, 600), Player = player };
            // player.Play();

            ////GeometryDrawing geometryDrawing = new GeometryDrawing(Brushes.Red,null, new RectangleGeometry(new Rect(0,0,800,600)));

            // DrawingBrush brush = new DrawingBrush(drawing);
            // Background = brush;

            //// Create a MediaTimeline.
            // MediaTimeline mTimeline =
            //    new MediaTimeline(new Uri("pack://application:,,,/Images/Niggacat.gif", UriKind.Absolute));

            //// Set the timeline to repeat.
            // mTimeline.RepeatBehavior = System.Windows.Media.Animation.RepeatBehavior.Forever;

            //// Create a clock from the MediaTimeline.
            // MediaClock mClock = mTimeline.CreateClock();

            // MediaPlayer repeatingVideoDrawingPlayer = new MediaPlayer();
            // repeatingVideoDrawingPlayer.Clock = mClock;

            // VideoDrawing repeatingVideoDrawing = new VideoDrawing();
            // repeatingVideoDrawing.Rect = new Rect(150, 0, 100, 100);
            // repeatingVideoDrawing.Player = repeatingVideoDrawingPlayer;
        }

        private void NewGameBtn_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mw = new MainWindow();
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
    }
}