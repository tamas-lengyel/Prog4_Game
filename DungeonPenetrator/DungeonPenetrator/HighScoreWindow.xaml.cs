// <copyright file="HighScoreWindow.xaml.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace DungeonPenetrator
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using Model;
    using Repository;

    /// <summary>
    /// Interaction logic for HighScoreWindow.xaml.
    /// </summary>
    public partial class HighScoreWindow : Window
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HighScoreWindow"/> class.
        /// </summary>
        public HighScoreWindow()
        {
            HighscoreRepository hsRepo = new HighscoreRepository();

            this.HighScores = hsRepo.GetAll().OrderByDescending(x => x.Level).ToList();

            this.InitializeComponent();

            if (this.HighScores.Count == 0)
            {
                MessageBox.Show("No highscores yeti!");
            }
            else
            {
                this.SetUpListBox();
            }
        }

        /// <summary>
        /// Gets the highscores.
        /// </summary>
        public List<Highscore> HighScores { get; private set; }

        private void SetUpListBox()
        {
            this.listBox.Items.Add("Character Name\t Levels Completed");
            foreach (var item in this.HighScores)
            {
                this.listBox.Items.Add($"{item.Name}: \t\t {item.Level}");
            }
        }
    }
}