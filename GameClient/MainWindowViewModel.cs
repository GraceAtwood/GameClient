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
