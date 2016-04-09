using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace GameClient
{
    //The backing view model of the main window.
    public class MainWindowViewModel : INotifyPropertyChanged
    {

        /// <summary>
        /// The locations of the application's database.
        /// </summary>
        public string DatabaseLocation
        {
            get
            {
                return GameBackend.ConnectionProvider.DatabaseLocation;
            }
            set
            {
                GameBackend.ConnectionProvider.DatabaseLocation = value;

                OnPropertyChanged("DatabaseLocation");
            }
        }

        /// <summary>
        /// Calls the backend to get all of the dialogue group IDs.
        /// </summary>
        public List<string> DialogueGroupIDs
        {
            get
            {
                return GameBackend.DialogueGroups.GetAllIDs();
            }
        }

        private GameBackend.DialogueGroups.DialogueGroup _selectedDialogueGroup = null;
        public GameBackend.DialogueGroups.DialogueGroup SelectedDialogueGroup
        {
            get
            {
                return _selectedDialogueGroup;
            }
            set
            {
                _selectedDialogueGroup = value;

                OnPropertyChanged("SelectedDialogueGroup");
            }
        }

        private string _newDialogueItem = null;
        public string NewDialogueItem
        {
            get
            {
                return _newDialogueItem;
            }
            set
            {
                _newDialogueItem = value;

                OnPropertyChanged("NewDialogueItem");
            }
        }

        /// <summary>
        /// Attempts to add a new dialogue group to the list and returns a boolean indicating success or failure.
        /// </summary>
        /// <param name="itemID"></param>
        /// <returns></returns>
        public async Task<bool> TryAddNewDialogueGroup(string itemID)
        {
            if (!string.IsNullOrWhiteSpace(itemID) && !DialogueGroupIDs.Contains(itemID))
            {
                await new GameBackend.DialogueGroups.DialogueGroup
                {
                    Elements = new System.Collections.ObjectModel.ObservableCollection<string>(new List<string>()),
                    ID = itemID
                }.DBInsert(true);

                OnPropertyChanged("DialogueGroupIDs");

                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Attempts to delete a dialogue group item and returns false if we can not.
        /// </summary>
        /// <param name="itemID"></param>
        /// <returns></returns>
        public async Task<bool> TryDeleteDialogueGroup(string itemID)
        {
            if (!string.IsNullOrWhiteSpace(itemID) && DialogueGroupIDs.Contains(itemID))
            {
                await new GameBackend.DialogueGroups.DialogueGroup
                {
                    ID = itemID
                }.DBDelete(true);

                OnPropertyChanged("DialogueGroupIDs");

                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// When the database connection has been updated.
        /// </summary>
        public async void OnDatabaseConnectionRefresh()
        {
            try
            {
                //Refresh the caches now that we have a new database.
                await GameBackend.DialogueGroups.InitializeCacheAsync();

                //Update any properties we care about
                OnPropertyChanged("DialogueGroupIDs");
            }
            catch
            {

                throw;
            }
        }

        /// <summary>
        /// Alerts listeners that a property has changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises the property changed event if it is not null.
        /// </summary>
        /// <param name="propertyName"></param>
        public void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
                
        }
    }
}
