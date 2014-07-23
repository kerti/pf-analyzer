using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using PFAnalyzer.Common;
using PFAnalyzer.DataModel;
using PFAnalyzer.Exceptions;

namespace PFAnalyzer.Extensions
{
    /// <summary>
    /// Extension class to contain the logic pertinent to the 
    /// <see cref="DataModel.PropertyDataModel"/> object model.
    /// </summary>
    public static class PropertyDataModelExtension
    {
        /// <summary>
        /// Initializes the lots collection of a <see cref="DataModel.PropertyDataModel"/> object.
        /// </summary>
        /// <remarks>
        /// Please note that this will automatically clear the lots collection.
        /// </remarks>
        /// <param name="model">
        /// The <see cref="DataModel.PropertyDataModel"/> object to be initialized.
        /// </param>
        public static void InitializeLots(this PropertyDataModel model)
        {
            model.Lots = new ObservableCollection<Lot>();
        }

        /// <summary>
        /// Initializes the costs collection of a <see cref="DataModel.PropertyDataModel"/> object.
        /// </summary>
        /// <remarks>
        /// Please note that this will automatically clear the costs collection.
        /// </remarks>
        /// <param name="model">
        /// The <see cref="DataModel.PropertyDataModel"/> object to be initialized.
        /// </param>
        public static void InitializeCosts(this PropertyDataModel model)
        {
            model.Costs = new ObservableCollection<Cost>();
        }

        /// <summary>
        /// Initialize a <see cref="DataModel.PropertyDataModel"/> object.
        /// </summary>
        /// <param name="model">
        /// The <see cref="DataModel.PropertyDataModel"/> object to be initialized.
        /// </param>
        public static void Initialize(this PropertyDataModel model)
        {
            model.InitializeLots();
            model.InitializeCosts();

            // TODO: Set the following defaults in a config file for configurability
            model.LandResaleProfitPercent = 10;
            model.ValueAddedTaxPercent = 5;
            model.FeePercent = (decimal)2.5;
            model.ProfitPoints = new decimal[] { 15, 20, 25, 30 };
            model.FinalProfitPercentage = 10;
        }

        /// <summary>
        /// Assign an event handler to the costs collection of a <see cref="DataModel.PropertyDataModel"/> object.
        /// </summary>
        /// <param name="model">
        /// The <see cref="DataModel.PropertyDataModel"/> object to receive the assignment.
        /// </param>
        /// <param name="handler">
        /// The <see cref="System.Collections.Specialized.NotifyCollectionChangedEventHandler"/> to be assigned.
        /// </param>
        public static void AssignCostsCollectionChangedEventHandler(this PropertyDataModel model,
            NotifyCollectionChangedEventHandler handler)
        {
            model.Costs.CollectionChanged += handler;
        }

        /// <summary>
        /// Recalculate the remaining land area to determine whether all available land
        /// area has been assigned to a specific function.
        /// </summary>
        /// <param name="model">
        /// The <see cref="DataModel.PropertyDataModel"/> object to be calculated.
        /// </param>
        /// <returns>
        /// The remaining land area, or area of land that has not been assigned to any function.
        /// </returns>
        public static decimal RecalculateRemainingLandArea(this PropertyDataModel model)
        {
            if (null == model.Lots)
            {
                model.Lots = new ObservableCollection<Lot>();
            }

            if (model.TotalLandArea > 0)
            {
                int totalAlottedLandArea = 0;
                foreach (Lot lot in model.Lots)
                {
                    totalAlottedLandArea += lot.LandArea;
                }

                return model.TotalLandArea - model.TotalRoadArea - model.TotalPublicFacilityArea - totalAlottedLandArea;
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// Add default costs that are considered common in the trade.
        /// </summary>
        /// <param name="model">
        /// The <see cref="DataModel.PropertyDataModel"/> to receive the costs.
        /// </param>
        /// <param name="clearBeforeAdd">
        /// If true, will clear all costs before adding new ones.
        /// </param>
        /// <param name="costsCollectionChangedEventHandler">
        /// The <see cref="System.Collections.Specialized.NotifyCollectionChangedEventHandler"/> to be assigned.
        /// </param>
        /// <param name="costPropertyChangedEventHandler">
        /// The <see cref="System.ComponentModel.PropertyChangedEventHandler"/> to be assigned.
        /// </param>
        public static void AddDefaultCosts(this PropertyDataModel model, bool clearBeforeAdd,
            NotifyCollectionChangedEventHandler costsCollectionChangedEventHandler,
            PropertyChangedEventHandler costPropertyChangedEventHandler)
        {
            if (null == model.Costs)
            {
                model.InitializeCosts();
                model.AssignCostsCollectionChangedEventHandler(costsCollectionChangedEventHandler);
            }

            if (clearBeforeAdd)
            {
                model.Costs.Clear();
            }

            List<string> names = (from c in model.Costs select c.Name).ToList();

            if (!names.Contains(Constants.COST_LAND_PURCHASE))
            {
                Cost cost1 = new Cost();
                cost1.Name = Constants.COST_LAND_PURCHASE;
                cost1.Quantity = model.TotalLandArea;
                cost1.Unit = "m²";
                cost1.UnitValue = model.BaseLandPrice;
                cost1.PropertyChanged += costPropertyChangedEventHandler;
                model.Costs.Add(cost1);
            }

            if (!names.Contains(Constants.COST_ROAD_PURCHASE))
            {
                Cost cost2 = new Cost();
                cost2.Name = Constants.COST_ROAD_PURCHASE;
                cost2.Quantity = model.TotalRoadArea;
                cost2.Unit = "m²";
                cost2.UnitValue = model.BaseLandPrice;
                cost2.PropertyChanged += costPropertyChangedEventHandler;
                model.Costs.Add(cost2);
            }

            if (!names.Contains(Constants.COST_PUBLIC_FACILITY))
            {
                Cost cost3 = new Cost();
                cost3.Name = Constants.COST_PUBLIC_FACILITY;
                cost3.Quantity = model.TotalPublicFacilityArea;
                cost3.Unit = "m²";
                cost3.UnitValue = model.BaseLandPrice;
                cost3.PropertyChanged += costPropertyChangedEventHandler;
                model.Costs.Add(cost3);
            }

            Cost cost4 = new Cost();
            cost4.Name = "Drainase";
            cost4.Quantity = 0;
            cost4.Unit = "m";
            cost4.UnitValue = 0;
            cost4.PropertyChanged += costPropertyChangedEventHandler;
            model.Costs.Add(cost4);

            Cost cost5 = new Cost();
            cost5.Name = "Resapan";
            cost5.Quantity = 0;
            cost5.Unit = "bh";
            cost5.UnitValue = 0;
            cost5.PropertyChanged += costPropertyChangedEventHandler;
            model.Costs.Add(cost5);

            Cost cost6 = new Cost();
            cost6.Name = "Urug";
            cost6.Quantity = 0;
            cost6.Unit = "ls";
            cost6.UnitValue = 0;
            cost6.PropertyChanged += costPropertyChangedEventHandler;
            model.Costs.Add(cost6);

            Cost cost7 = new Cost();
            cost7.Name = "Pembatas Kavling";
            cost7.Quantity = 0;
            cost7.Unit = "m";
            cost7.UnitValue = 0;
            cost7.PropertyChanged += costPropertyChangedEventHandler;
            model.Costs.Add(cost7);

            Cost cost8 = new Cost();
            cost8.Name = "Kontribusi Wilayah";
            cost8.Quantity = 0;
            cost8.Unit = "unit";
            cost8.UnitValue = 0;
            cost8.PropertyChanged += costPropertyChangedEventHandler;
            model.Costs.Add(cost8);

            Cost cost9 = new Cost();
            cost9.Name = "Biaya Pecah";
            cost9.Quantity = 0;
            cost9.Unit = "bh";
            cost9.UnitValue = 0;
            cost9.PropertyChanged += costPropertyChangedEventHandler;
            model.Costs.Add(cost9);

            Cost cost10 = new Cost();
            cost10.Name = "AJB";
            cost10.Quantity = 0;
            cost10.Unit = "bh";
            cost10.UnitValue = 0;
            cost10.PropertyChanged += costPropertyChangedEventHandler;
            model.Costs.Add(cost10);
        }

        /// <summary>
        /// Validates everything in the <see cref="DataModel.PropertyDataModel"/> object.
        /// </summary>
        /// <param name="model">
        /// The <see cref="DataModel.PropertyDataModel"/> object to be validated.
        /// </param>
        /// <exception cref="Exceptions.DataValidationException">
        /// Thrown when the model does not pass validation.
        /// </exception>
        public static void Validate(this PropertyDataModel model)
        {
            // verify property variables are complete
            if (string.IsNullOrEmpty(model.Location))
            {
                throw new DataValidationException("Lokasi properti belum ditentukan.");
            }

            if (0 == model.TotalLandArea)
            {
                throw new DataValidationException("Luas total properti belum ditentukan.");
            }

            if (0 == model.TotalRoadArea)
            {
                // TODO: This shouldn't be a problem since a block of properties may not have inner roads on its own.
            }

            if (0 == model.BaseLandPrice)
            {
                throw new DataValidationException("Harga dasar tanah belum ditentukan.");
            }

            if (0 == model.BuildingPrice)
            {
                throw new DataValidationException("Harga bangunan belum ditentukan.");
            }

            // verify base prices and costs are complete
            if (0 == model.LandResaleProfitPercent)
            {
                // TODO: This shouldn't be a problem since theoretically it's possible to sell the land without profit.
            }

            if (0 == model.BuildingPermitCostPerLot)
            {
                throw new DataValidationException("Biaya IMB per kavling belum ditentukan.");
            }

            if (0 == model.PromoCostPerLot)
            {
                // TODO: This shouldn't be a problem since theoretically it's possible to defer promo costs to another budget.
            }

            if (0 == model.FeePercent)
            {
                throw new DataValidationException("Persen fee belum ditentukan.");
            }

            // verify at least one lot is created
            if (null == model.Lots)
            {
                model.Lots = new ObservableCollection<Lot>();
                throw new DataValidationException("Belum ada kavling yang ditentukan.");
            }
            else if (0 == model.Lots.Count)
            {
                throw new DataValidationException("Belum ada kavling yang ditentukan.");
            }

            // verify all land area has been allocated
            if (model.RecalculateRemainingLandArea() > 0)
            {
                throw new DataValidationException("Belum semua luasan tanah telah terpakai baik untuk jalan lingkungan atau untuk kavling.");
            }

            // verify all lots have complete initial variables
            foreach (Lot lot in model.Lots)
            {
                if (string.IsNullOrEmpty(lot.Name))
                {
                    throw new DataValidationException("Salah satu nama kavling masih kosong.");
                }

                if (0 == lot.LandArea)
                {
                    throw new DataValidationException("Luas tanah untuk kavling \"" + lot.Name + "\" belum ditentukan."
                        + "\n\nApabila Anda tidak ingin memperhitungkan luasan kavling tersebut, silakan hapus dari daftar kavling.");
                }

                if (0 == lot.BuildingArea)
                {
                    // TODO: This shouldn't be a problem since a lot may be sold as land only with no buildings planned.
                }
            }

            // verify at least one cost is created
            if (null == model.Costs || 0 == model.Costs.Count)
            {
                throw new DataValidationException("Biaya-biaya belum ditentukan.");
            }

            // verify land purchase cost, road purchase cost, and public facility area purchase cost are created
            List<string> costNames = (from c in model.Costs select c.Name).ToList();

            if (!costNames.Contains(Constants.COST_LAND_PURCHASE))
            {
                throw new DataValidationException("Biaya pembelian tanah belum ditentukan.");
            }

            if (model.TotalRoadArea > 0 && !costNames.Contains(Constants.COST_ROAD_PURCHASE))
            {
                throw new DataValidationException("Biaya pembelian tanah untuk jalan lingkungan belum ditentukan.");
            }

            if (model.TotalPublicFacilityArea > 0 && !costNames.Contains(Constants.COST_PUBLIC_FACILITY))
            {
                throw new DataValidationException("Biaya pembelian tanah untuk fasilitas umum belum ditentukan.");
            }

            // verify all costs have proper unit value, quantity, and total value set
            foreach (Cost cost in model.Costs)
            {
                if (0 == cost.UnitValue)
                {
                    throw new DataValidationException("Harga satuan belum ditentukan untuk biaya/pekerjaan \"" + cost.Name + "\"."
                        + "\n\nApabila Anda tidak ingin memperhitungkan biaya/pekerjaan tersebut, silakan hapus dari daftar biaya.");
                }

                if (0 == cost.Quantity)
                {
                    throw new DataValidationException("Volume belum ditentukan untuk biaya/pekerjaan \"" + cost.Name + "\"."
                        + "\n\nApabila Anda tidak ingin memperhitungkan biaya/pekerjaan tersebut, silakan hapus dari daftar biaya.");
                }

                if (0 == cost.TotalValue)
                {
                    throw new DataValidationException("Biaya total belum ditentukan untuk biaya/pekerjaan \"" + cost.Name + "\"."
                        + "\n\nApabila Anda tidak ingin memperhitungkan biaya/pekerjaan tersebut, silakan hapus dari daftar biaya.");
                }
            }
        }

        /// <summary>
        /// The pinnacle of this application, this is where the magic happens.
        /// </summary>
        /// <param name="model">
        /// The <see cref="DataModel.PropertyDataModel"/> object to be calculated.
        /// </param>
        public static void Calculate(this PropertyDataModel model)
        {
            // calculate total cost of development = sum of all cost total values
            model.TotalCostsOfDevelopment = 0;
            foreach (Cost cost in model.Costs)
            {
                model.TotalCostsOfDevelopment += cost.TotalValue;
            }

            // calculate effective land cost = total cost of development / (total land area - total road area)
            model.EffectiveLandCost = model.TotalCostsOfDevelopment / (model.TotalLandArea - (model.TotalRoadArea + model.TotalPublicFacilityArea));

            // calculate land resale price = effective land cost * (1 + (land resale profit in percent / 100))
            model.LandResalePrice = model.EffectiveLandCost * (1 + (model.LandResaleProfitPercent / 100));

            // for each lot
            model.TotalBaseSalePrice = 0;
            foreach (Lot lot in model.Lots)
            {
                // set common costs
                lot.BuildingPermitCost = model.BuildingPermitCostPerLot;
                lot.PromoCost = model.PromoCostPerLot;

                // calculate total building price = building price * building area
                lot.TotalBuildingCost = model.BuildingPrice * lot.BuildingArea;

                // calculate total land price = land resale price * total land area
                lot.TotalLandCost = model.LandResalePrice * lot.LandArea;

                // calculate total nett price = total building price
                // + total land price + building permit + promo cost
                lot.TotalNettPrice = lot.TotalBuildingCost + lot.TotalLandCost
                    + model.BuildingPermitCostPerLot + model.PromoCostPerLot;

                // calculate value added tax = %VAT * total nett price
                lot.ValueAddedTax = (model.ValueAddedTaxPercent / 100) * lot.TotalNettPrice;

                // calculate fee = %fee * total nett price
                lot.Fee = (model.FeePercent / 100) * lot.TotalNettPrice;

                // calculate base sale price = total nett price + VAT + fee
                lot.BaseSalePrice = lot.TotalNettPrice + lot.ValueAddedTax + lot.Fee;

                // calculate price points
                lot.PricePoints = new ObservableCollection<PricePoint>();
                for (int i = 0; i < model.ProfitPoints.Count(); i++)
                {
                    PricePoint pricePoint = new PricePoint();
                    pricePoint.BaseSalePrice = lot.BaseSalePrice;
                    pricePoint.ProfitAssumptionPercent = model.ProfitPoints[i];
                    pricePoint.ProfitNominal = pricePoint.BaseSalePrice * (pricePoint.ProfitAssumptionPercent / 100);
                    pricePoint.FinalPriceNominal = pricePoint.BaseSalePrice + pricePoint.ProfitNominal;
                    lot.PricePoints.Add(pricePoint);
                }

                // calculate sum of base sale price
                model.TotalBaseSalePrice += lot.BaseSalePrice;
            }

            // calculate final profit = total base sale price * final profit percentage / 100
            model.FinalProfitNominal = model.TotalBaseSalePrice * (model.FinalProfitPercentage / 100);

            // calculate total actual land value = total land purchase cost + final profit
            model.TotalActualLandValue = 0;
            foreach (Cost cost in model.Costs)
            {
                if (Constants.COST_LAND_PURCHASE.Equals(cost.Name))
                {
                    model.TotalActualLandValue = cost.TotalValue + model.FinalProfitNominal;
                    break;
                }
            }

            // calculate actual land value = total actual land value / total land area
            model.ActualLandValue = model.TotalActualLandValue / model.TotalLandArea;
        }

        /// <summary>
        /// Convert a <see cref="DataModel.PropertyDataModel"/> object to an
        /// XML-formatted string.
        /// </summary>
        /// <param name="model">
        /// The object to convert.
        /// </param>
        /// <returns>
        /// The resulting XML-formatted string.
        /// </returns>
        public static string ToXML(this PropertyDataModel model)
        {
            var stringwriter = new StringWriter();
            var serializer = new XmlSerializer(model.GetType());
            serializer.Serialize(stringwriter, model);
            return stringwriter.ToString();
        }

        /// <summary>
        /// Convert an XML-formatted string to a
        /// <see cref="DataModel.PropertyDataModel"/> object.
        /// </summary>
        /// <param name="model">
        /// The object model in which the resulting data should be placed.
        /// </param>
        /// <param name="xml">
        /// The XML-formatted string to convert.
        /// </param>
        /// <returns>
        /// The generated <see cref="DataModel.PropertyDataModel"/> object.
        /// </returns>
        public static PropertyDataModel FromXML(this PropertyDataModel model, string xml)
        {
            var stringReader = new StringReader(xml);
            var serializer = new XmlSerializer(typeof(PropertyDataModel));
            return serializer.Deserialize(stringReader) as PropertyDataModel;
        }
    }
}
