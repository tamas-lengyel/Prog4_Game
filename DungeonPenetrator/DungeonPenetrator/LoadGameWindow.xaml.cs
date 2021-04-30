using Model;
using Repository;
using System;
using System.Windows;
using System.Windows.Forms;

namespace DungeonPenetrator
{
    /// <summary>
    /// Interaction logic for LoadGameWindow.xaml
    /// </summary>
    public partial class LoadGameWindow : Window
    {
        public string FilePath{get;set;}

        public LoadGameWindow()
        {
            InitializeComponent();
        }

        private void btnOpenFile_Click(object sender, RoutedEventArgs e)
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
                this.FilePath = dlg.FileName;
                this.DialogResult = true;
                this.Close();
            }

        }
    }
}