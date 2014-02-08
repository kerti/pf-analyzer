using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pf_analyzer.DataModel
{

    /// <summary>
    /// This class represents all costs associated with building houses on
    /// every lot in a property.
    /// </summary>
    class Cost : BaseModel
    {

        #region Public Properties

        /// <summary>
        /// The name or label of this cost.
        /// </summary>
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
            }
        }
        /// <summary>
        /// How much of this is required.
        /// </summary>
        public decimal Quantity
        {
            get
            {
                return quantity;
            }
            set
            {
                quantity = value;
                totalValue = unitValue * value;
                NotifyPropertyChanged("TotalValue");
            }
        }

        /// <summary>
        /// The units represented in the calculation.
        /// </summary>
        public string Unit
        {
            get
            {
                return unit;
            }
            set
            {
                unit = value;
            }
        }

        /// <summary>
        /// The value of one unit of this cost.
        /// </summary>
        public decimal UnitValue {
            get
            {
                return unitValue;
            }
            set
            {
                unitValue = value;
                totalValue = value * quantity;
                NotifyPropertyChanged("TotalValue");
            }
        }

        /// <summary>
        /// The value of all units of this cost.
        /// </summary>
        public decimal TotalValue
        {
            get
            {
                return totalValue;
            }
            set
            {
                totalValue = value;
                unitValue = value / quantity;
                NotifyPropertyChanged("UnitValue");
            }
        }

        #endregion

        #region Private Properties

        /// <summary>
        /// Private property for name or label of this cost.
        /// </summary>
        private string name;

        /// <summary>
        /// Private property for how much of this is required.
        /// </summary>
        private decimal quantity;

        /// <summary>
        /// Private property for the units represented in the calculation.
        /// </summary>
        private string unit;
        
        /// <summary>
        /// Private property for the value of the unit of this cost.
        /// </summary>
        private decimal unitValue;

        /// <summary>
        /// Private property for the value of all units of this cost.
        /// </summary>
        private decimal totalValue;

        #endregion


    }
}
