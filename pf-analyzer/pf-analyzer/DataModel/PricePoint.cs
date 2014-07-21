
namespace pf_analyzer.DataModel
{
    /// <summary>
    /// This class represents a price point at which a lot is to be sold.
    /// </summary>
    public class PricePoint : BaseModel
    {

        /// <summary>
        /// The price which will be the base for the calculation of this price point.
        /// </summary>
        public decimal BaseSalePrice { get; set; }

        /// <summary>
        /// The assumed percentage of profit expected from the sales of this lot.
        /// </summary>
        public decimal ProfitAssumptionPercent { get; set; }

        /// <summary>
        /// The nominal value of the profit.
        /// </summary>
        public decimal ProfitNominal { get; set; }

        /// <summary>
        /// The nominal value of the final selling price.
        /// </summary>
        public decimal FinalPriceNominal { get; set; }

    }
}
