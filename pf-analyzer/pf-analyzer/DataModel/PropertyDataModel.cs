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

        public decimal TotalCostsOfDevelopment { get; set; }

        public decimal EffectiveLandCost { get; set; }

        public decimal LandResaleProfitPercent { get; set; }

        public decimal LandResalePrice { get; set; }

        public decimal BuildingPrice { get; set; }

        public decimal BuildingPermitCostPerLot { get; set; }

        public decimal PromoCostPerLot { get; set; }

        public decimal ValueAddedTaxPercent { get; set; }

        public decimal FeePercent { get; set; }

        public decimal[] ProfitPoints { get; set; }

        public decimal TotalBaseSalePrice { get; set; }

        public decimal FinalProfitPercentage { get; set; }

        public decimal FinalProfitNominal { get; set; }

        public decimal TotalActualLandValue { get; set; }

        public decimal ActualLandValue { get; set; }
    }
}