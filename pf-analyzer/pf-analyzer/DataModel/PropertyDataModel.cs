using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pf_analyzer.DataModel
{
    /// <summary>
    /// This class represents the all the data associated with a
    /// property that is to be analyzed.
    /// </summary>
    class PropertyDataModel : BaseModel
    {
        /// <summary>
        /// The location of the property. Could be a name or an address or an
        /// abbreviated address.
        /// </summary>
        public string Location { get; set; }

        /// <summary>
        /// Total land area that is workable.
        /// </summary>
        public decimal TotalLandArea { get; set; }

        /// <summary>
        /// Total land area set aside for public roads.
        /// </summary>
        public decimal TotalRoadArea { get; set; }

        /// <summary>
        /// Total land area set aside for public facilities such as parks or recreation areas.
        /// </summary>
        public decimal TotalPublicFacilityArea { get; set; }

        /// <summary>
        /// The price of the property per meter squared as set by the seller.
        /// </summary>
        public decimal BaseLandPrice { get; set; }

        /// <summary>
        /// The collection of lots associated with this piece of property.
        /// </summary>
        public ObservableCollection<Lot> Lots { get; set; }

        /// <summary>
        /// The collection of costs associated with this piece of property.
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