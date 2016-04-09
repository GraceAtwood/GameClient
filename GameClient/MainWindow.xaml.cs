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
using System.Windows.Media.Animation;
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

                //Let's trigger a refresh in the view model.
                (this.DataContext as MainWindowViewModel).OnDatabaseConnectionRefresh();
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

        private void ListBox_DialogueGroups_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            (this.DataContext as MainWindowViewModel).SelectedDialogueGroup = GameBackend.DialogueGroups.Fetch((sender as ListBox).SelectedItem as string);
        }

        private async void Button_AddNewDialogueGroupItem_Click(object sender, RoutedEventArgs e)
        {
            //Call the view model's add new method and see if it succeeds.  If it doesn't, flash the field to the user.
            bool success = await (this.DataContext as MainWindowViewModel).TryAddNewDialogueGroup((this.DataContext as MainWindowViewModel).NewDialogueItem);

            if (!success)
            {
                var blinkAnimation = TextBox_NewDialogueItem.FindResource("Storyboard_NewDialogueGroupFlash") as Storyboard;

                if (blinkAnimation != null)
                    blinkAnimation.Begin();
            }
            else
            {
                ListBox_DialogueGroups.SelectedItem = (this.DataContext as MainWindowViewModel).NewDialogueItem;
                (this.DataContext as MainWindowViewModel).NewDialogueItem = null;
            }
                
        }

        private async void Button_DeleteDialogueGroup_Click(object sender, RoutedEventArgs e)
        {
            string groupID = ((sender as Button).TemplatedParent as ContentPresenter).DataContext as string;

            if (!string.IsNullOrWhiteSpace(groupID))
                //Call the view model's delete method
                await (this.DataContext as MainWindowViewModel).TryDeleteDialogueGroup(groupID);
        }

        private async void Button_DeleteDialogueElement_Click(object sender, RoutedEventArgs e)
        {
            ListBoxItem item = (ListBoxItem)ListBox_DialogueElements.ItemContainerGenerator.ContainerFromItem((sender as Button).DataContext);

            int index = ListBox_DialogueElements.ItemContainerGenerator.IndexFromContainer(item);

            (this.DataContext as MainWindowViewModel).SelectedDialogueGroup.Elements.RemoveAt(index);

            //And then update it
            await (this.DataContext as MainWindowViewModel).SelectedDialogueGroup.DBUpdate(true);
        }

        private async void Button_AddNewDialogueElement_Click(object sender, RoutedEventArgs e)
        {
            string text = TextBox_NewDialogueElement.Text;

            (this.DataContext as MainWindowViewModel).SelectedDialogueGroup.Elements.Add(text);

            //And then update the group
            await (this.DataContext as MainWindowViewModel).SelectedDialogueGroup.DBUpdate(true);
        }
    }
}
