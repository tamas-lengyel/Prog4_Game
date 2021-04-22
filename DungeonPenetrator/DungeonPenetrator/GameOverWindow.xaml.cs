using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Repository;
using Model;
using Logic;

namespace DungeonPenetrator
{
    /// <summary>
    /// Interaction logic for GameOverWindow.xaml
    /// </summary>
    public partial class GameOverWindow : Window
    {
        private ISaveGameRepository sgRepo;
        private ILoadingLogic logic;
        public GameOverWindow(ILoadingLogic logic, ISaveGameRepository sgRepo)
        {
            this.logic = logic;
            this.sgRepo = sgRepo;
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            logic.EndGame(input.Text);

            MainMenu menu = new MainMenu();
            menu.Show();
            this.Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            sgRepo.Insert(default);
            MainMenu menu = new MainMenu();
            menu.Show();
            this.Close();
        }
    }
}
