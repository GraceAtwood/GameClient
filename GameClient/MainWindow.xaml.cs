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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GameClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

       
        private async void Button_FindDatabase_Click(object sender, RoutedEventArgs e)
        {
            //First ask the client to find a database for us.
            Microsoft.Win32.OpenFileDialog dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.ShowDialog();

            //We're going to pass the result into the view model now.
            (this.DataContext as MainWindowViewModel).DatabaseLocation = dialog.FileName;

            //And finally, we're going to ask the backend if the database is valid and change the text block status indicator accordingly
            if (await GameBackend.Diagnostics.TestDatabaseConnectionAsync((this.DataContext as MainWindowViewModel).DatabaseLocation))
            {
                //The connection was valid. Woohoo
                TextBlock_DatabaseStatus.Text = "Database Valid";
                TextBlock_DatabaseStatus.Background = new SolidColorBrush(Colors.Green);
            }
            else
            {
                // :(  so sad, no connection
                TextBlock_DatabaseStatus.Text = "Database Invalid";
                TextBlock_DatabaseStatus.Background = new SolidColorBrush(Colors.Red);
            }
        }

        private void Button_Settings_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
