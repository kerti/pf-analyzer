namespace PFAnalyzer.DataModel
{
    /// <summary>
    /// This class represents a price point at which a lot is to be sold.
    /// </summary>
    public class PricePoint : BaseModel
    {
        /// <summary>
        /// Gets or sets the price which will be the base for the calculation of this price point.
        /// </summary>
        public decimal BaseSalePrice { get; set; }

        /// <summary>
        /// Gets or sets the assumed percentage of profit expected from the sales of this lot.
        /// </summary>
        public decimal ProfitAssumptionPercent { get; set; }

        /// <summary>
        /// Gets or sets the nominal value of the profit.
        /// </summary>
        public decimal ProfitNominal { get; set; }

        /// <summary>
        /// Gets or sets the nominal value of the final selling price.
        /// </summary>
        public decimal FinalPriceNominal { get; set; }
    }
}
