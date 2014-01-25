using System;
using System.ComponentModel;

namespace pf_analyzer.DataModel
{
    class BaseModel : INotifyPropertyChanged
    {

        #region Properties

        /// <summary>
        /// The public event handler for notifying that a property has changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Methods

        /// <summary>
        /// Invoke thw PropertyChanged event to notify that a property
        /// has changed.
        /// </summary>
        /// <param name="propertyName"></param>
        protected void NotifyPropertyChanged(String propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

    }
}
