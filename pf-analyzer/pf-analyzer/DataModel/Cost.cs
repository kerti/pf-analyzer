namespace PFAnalyzer.DataModel
{
    /// <summary>
    /// This class represents all costs associated with building houses on
    /// every lot in a property.
    /// </summary>
    public class Cost : BaseModel
    {
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

        #region Public Properties

        /// <summary>
        /// Gets or sets the name or label of this cost.
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
        /// Gets or sets the quantity of this cost item.
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
                NotifyPropertyChanged("Quantity");
                totalValue = unitValue * value;
                NotifyPropertyChanged("TotalValue");
            }
        }

        /// <summary>
        /// Gets or sets the units represented in the calculation.
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
        /// Gets or sets the value of one unit of this cost.
        /// </summary>
        public decimal UnitValue {
            get
            {
                return unitValue;
            }

            set
            {
                unitValue = value;
                NotifyPropertyChanged("UnitValue");
                totalValue = value * quantity;
                NotifyPropertyChanged("TotalValue");
            }
        }

        /// <summary>
        /// Gets or sets the total value.
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
                NotifyPropertyChanged("TotalValue");
                if (quantity > 0)
                {
                    unitValue = value / quantity;
                }
                else
                {
                    unitValue = 0;
                }

                NotifyPropertyChanged("UnitValue");
            }
        }

        #endregion
    }
}
