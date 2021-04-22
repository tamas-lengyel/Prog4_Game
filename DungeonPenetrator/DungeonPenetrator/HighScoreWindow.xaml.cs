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

namespace DungeonPenetrator
{
    /// <summary>
    /// Interaction logic for HighScoreWindow.xaml
    /// </summary>
    public partial class HighScoreWindow : Window
    {
        public List<Highscore> HighScores { get; private set; }

        public HighScoreWindow()
        {
            HighscoreRepository hsRepo = new HighscoreRepository();
            
            HighScores = hsRepo.GetAll().OrderByDescending(x=>x.Level).ToList();

            InitializeComponent();
            if (this.HighScores.Count == 0)
            {
                MessageBox.Show("No highscores yeti!");
            }
            else
            {
                this.SetUpListBox();
            }
        }

        private void SetUpListBox()
        {
            this.listBox.Items.Add("Character Name\t Levels Completed");
            foreach (var item in HighScores)
            {
                listBox.Items.Add($"{item.Name}: \t\t {item.Level}");
            }
        }
    }
}
