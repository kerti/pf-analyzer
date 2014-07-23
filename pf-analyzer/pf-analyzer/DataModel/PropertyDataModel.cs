using System.Collections.ObjectModel;

namespace PFAnalyzer.DataModel
{
    /// <summary>
    /// This class represents the all the data associated with a
    /// property that is to be analyzed.
    /// </summary>
    public class PropertyDataModel : BaseModel
    {
        /// <summary>
        /// Gets or sets the location of the property. Could be a name or an address or an
        /// abbreviated address.
        /// </summary>
        public string Location { get; set; }

        /// <summary>
        /// Gets or sets the total land area that is workable.
        /// </summary>
        public decimal TotalLandArea { get; set; }

        /// <summary>
        /// Gets or sets the total land area set aside for public roads.
        /// </summary>
        public decimal TotalRoadArea { get; set; }

        /// <summary>
        /// Gets or sets the total land area set aside for public facilities such as parks or recreation areas.
        /// </summary>
        public decimal TotalPublicFacilityArea { get; set; }

        /// <summary>
        /// Gets or sets the price of the property per meter squared as set by the seller.
        /// </summary>
        public decimal BaseLandPrice { get; set; }

        /// <summary>
        /// Gets or sets the collection of lots associated with this piece of property.
        /// </summary>
        public ObservableCollection<Lot> Lots { get; set; }

        /// <summary>
        /// Gets or sets the collection of costs associated with this piece of property.
        /// </summary>
        public ObservableCollection<Cost> Costs { get; set; }

        /// <summary>
        /// Gets or sets the total cost of development.
        /// </summary>
        public decimal TotalCostsOfDevelopment { get; set; }

        /// <summary>
        /// Gets or sets the effective land cost.
        /// </summary>
        public decimal EffectiveLandCost { get; set; }

        /// <summary>
        /// Gets or sets the land resale profit percentage point.
        /// </summary>
        public decimal LandResaleProfitPercent { get; set; }

        /// <summary>
        /// Gets or sets the land resale price.
        /// </summary>
        public decimal LandResalePrice { get; set; }

        /// <summary>
        /// Gets or sets the building price.
        /// </summary>
        public decimal BuildingPrice { get; set; }

        /// <summary>
        /// Gets or sets the building permit cost per lot.
        /// </summary>
        public decimal BuildingPermitCostPerLot { get; set; }

        /// <summary>
        /// Gets or sets the promo cost per lot.
        /// </summary>
        public decimal PromoCostPerLot { get; set; }

        /// <summary>
        /// Gets or sets the Value Added Tax percentage point.
        /// </summary>
        public decimal ValueAddedTaxPercent { get; set; }

        /// <summary>
        /// Gets or sets the fee percentage point.
        /// </summary>
        public decimal FeePercent { get; set; }

        /// <summary>
        /// Gets or sets the profit percentage points to be included in the final report.
        /// </summary>
        public decimal[] ProfitPoints { get; set; }

        /// <summary>
        /// Gets or sets the total base sale price of the property.
        /// </summary>
        public decimal TotalBaseSalePrice { get; set; }

        /// <summary>
        /// Gets or sets the final profit percentage point.
        /// </summary>
        public decimal FinalProfitPercentage { get; set; }

        /// <summary>
        /// Gets or sets the final profit nominal value.
        /// </summary>
        public decimal FinalProfitNominal { get; set; }

        /// <summary>
        /// Gets or sets the total actual land value.
        /// </summary>
        public decimal TotalActualLandValue { get; set; }

        /// <summary>
        /// Gets or sets the actual land value per meter.
        /// </summary>
        public decimal ActualLandValue { get; set; }
    }
}