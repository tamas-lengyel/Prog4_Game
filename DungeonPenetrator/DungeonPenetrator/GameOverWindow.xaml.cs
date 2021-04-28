// <copyright file="GameOverWindow.xaml.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace DungeonPenetrator
{
    using System.Windows;
    using Logic;
    using Repository;

    /// <summary>
    /// Interaction logic for GameOverWindow.xaml.
    /// </summary>
    public partial class GameOverWindow : Window
    {
        private ISaveGameRepository sgRepo;
        private ILoadingLogic logic;

        /// <summary>
        /// Initializes a new instance of the <see cref="GameOverWindow"/> class.
        /// </summary>
        /// <param name="logic">loadinglogic repo.</param>
        /// <param name="sgRepo">savegame repo.</param>
        public GameOverWindow(ILoadingLogic logic, ISaveGameRepository sgRepo)
        {
            this.logic = logic;
            this.sgRepo = sgRepo;
            this.InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.logic.EndGame(this.input.Text);

            MainMenu menu = new MainMenu();
            menu.Show();
            this.Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.sgRepo.Insert(default);
            MainMenu menu = new MainMenu();
            menu.Show();
            this.Close();
        }
    }
}