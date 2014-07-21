using System.Collections.ObjectModel;

namespace pf_analyzer.DataModel
{
    /// <summary>
    /// This class represents a lot.
    /// </summary>
    public class Lot : BaseModel
    {
        /// <summary>
        /// The name of the lot.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The total area of the lot. This includes the area used up for buildings.
        /// </summary>
        public int LandArea { get; set; }

        /// <summary>
        /// The total area of the lot used up for buildings.
        /// This does not include land area that is not built on.
        /// </summary>
        public int BuildingArea { get; set; }

        /// <summary>
        /// The total cost of the building for this lot.
        /// </summary>
        public decimal TotalBuildingCost { get; set; }

        /// <summary>
        /// The total cost of land for this lot.
        /// </summary>
        public decimal TotalLandCost { get; set; }

        /// <summary>
        /// The cost of obtaining a building permit for this lot.
        /// </summary>
        public decimal BuildingPermitCost { get; set; }

        /// <summary>
        /// The cost of promoting and advertising this lot.
        /// </summary>
        public decimal PromoCost { get; set; }

        /// <summary>
        /// The total nett price of this lot, which is calculated as
        /// total building cost + total land cost + building permit cost + promo cost
        /// </summary>
        public decimal TotalNettPrice { get; set; }

        /// <summary>
        /// Value Added Tax applied for this lot, calculated against total nett price.
        /// </summary>
        public decimal ValueAddedTax { get; set; }

        /// <summary>
        /// Fee applied for this lot, calculated against total nett price.
        /// </summary>
        public decimal Fee { get; set; }

        /// <summary>
        /// The base sale price for this lot, calculated as
        /// total nett price + VAT + fee
        /// </summary>
        public decimal BaseSalePrice { get; set; }

        /// <summary>
        /// A collection of price points that the lot is expected to be sold for.
        /// </summary>
        public ObservableCollection<PricePoint> PricePoints { get; set; }

        
    }
}
