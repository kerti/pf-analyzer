using System.ComponentModel;

namespace PFAnalyzer.DataModel
{
    public class BaseModel : INotifyPropertyChanged
    {
        #region Properties

        /// <summary>
        /// The public event handler for notifying that a property has changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Methods

        /// <summary>
        /// Invoke the PropertyChanged event to notify that a property
        /// has changed.
        /// </summary>
        /// <param name="propertyName">
        /// The name of the property that is changed.
        /// </param>
        protected void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion
    }
}
