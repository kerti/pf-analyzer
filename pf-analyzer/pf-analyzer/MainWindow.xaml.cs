using MahApps.Metro.Controls;
using pf_analyzer.DataModel;
using pf_analyzer.Exceptions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace pf_analyzer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {

        #region Private Properties

        private PropertyDataModel data;
        private static readonly string COST_LAND_PURCHASE = "Harga Beli Tanah";
        private static readonly string COST_ROAD_PURCHASE = "Biaya Tanah untuk Jalan";

        #endregion

        public MainWindow()
        {
            InitializeComponent();

            // Create cellstyle
            Style cellStyle = new Style(typeof(DataGridCell));

            // If a cell is editing the border should be red
            Trigger isEditingTrigger = new Trigger();
            isEditingTrigger.Property = DataGridCell.IsEditingProperty;
            isEditingTrigger.Value = true;
            isEditingTrigger.Setters.Add(new Setter(DataGridCell.BorderBrushProperty, Brushes.Red));

            cellStyle.Triggers.Add(isEditingTrigger);

            // Set the cell style for the grid
            this.dgBasicLots.CellStyle = cellStyle;


            InitializeDataModel();
            InitializeDummyData();
            this.DataContext = data;
            RecalculateRemainingLandArea();
        }

        #region Dummy Data

        private void InitializeDataModel()
        {
            data = new PropertyDataModel();
            InitializeCosts();
            data.LandResaleProfitPercent = 10;
            data.ValueAddedTaxPercent = 5;
            data.FeePercent = (decimal)2.5;
            data.ProfitPoints = new decimal[] { 15, 20, 25, 30 };
            data.FinalProfitPercentage = 10;
        }

        private void InitializeDummyData()
        {
            data.Location = "Karangwaru";
            data.TotalLandArea = 609;
            data.TotalRoadArea = 138;
            data.BaseLandPrice = 1100000;
            data.Lots = new ObservableCollection<Lot>();

            Lot lot1 = new Lot();
            lot1.Name = "Kavling A";
            lot1.LandArea = 158;
            lot1.BuildingArea = 60;
            data.Lots.Add(lot1);

            Lot lot2 = new Lot();
            lot2.Name = "Kavling B";
            lot2.LandArea = 81;
            lot2.BuildingArea = 45;
            data.Lots.Add(lot2);

            Lot lot3 = new Lot();
            lot3.Name = "Kavling C";
            lot3.LandArea = 90;
            lot3.BuildingArea = 45;
            data.Lots.Add(lot3);

            Lot lot4 = new Lot();
            lot4.Name = "Kavling D";
            lot4.LandArea = 142;
            lot4.BuildingArea = 60;
            data.Lots.Add(lot4);

            data.BuildingPrice = 1750000;
            data.BuildingPermitCostPerLot = 2500000;
            data.PromoCostPerLot = 2500000;
        }

        #endregion

        #region Control Events

        #region Common Control Events

        private void TextboxGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            (e.OriginalSource as TextBox).SelectAll();
        }

        private void TextboxGotMouseCapture(object sender, MouseEventArgs e)
        {
            (e.OriginalSource as TextBox).SelectAll();
        }

        #endregion

        #region Page One

        private void HandleSetRemainingLandAreaLabel(object sender, DataGridCellEditEndingEventArgs e)
        {
            SetRemainingLandAreaLabel();
        }

        private void HandleSetRemainingLandAreaLabel(object sender, RoutedEventArgs e)
        {
            SetRemainingLandAreaLabel();
        }

        private void SetRemainingLandAreaLabel()
        {
            lblRemainingAreaNominal.Content = RecalculateRemainingLandArea().ToString();
        }

        private void DeleteLot(object sender, RoutedEventArgs e)
        {
            Lot lot = dgBasicLots.SelectedItem as Lot;
            if (null != lot)
            {
                // TODO: Use metro-styled MessageBox for theme consistency
                MessageBoxResult answer = MessageBox.Show(
                    "Apakah Anda yakin ingin menghapus " + lot.Name + "?", "Konfirmasi Menghapus Kavling", MessageBoxButton.OKCancel);
                if (MessageBoxResult.OK == answer)
                {
                    data.Lots.Remove(lot);
                }
            }
            else
            {
                // TODO: Use metro-styled MessageBox for theme consistency
                MessageBox.Show("Silakan pilih dahulu kavling yang akan dihapus.", "Pilih Kavling");
            }
        }

        private void FirstPageNext(object sender, RoutedEventArgs e)
        {
            if (ValidateFirstPage())
            {
                matcPrimaryTabControl.SelectedIndex = 1;
            }
        }

        #endregion

        #region Page Two

        private void AddDefaultCosts(object sender, RoutedEventArgs e)
        {

            if (null == data.Costs)
            {
                InitializeCosts();
            }

            if (data.Costs.Count > 0)
            {
                // TODO: Use metro-styled MessageBox for theme consistency
                MessageBoxResult answer = MessageBox.Show(
                    "Anda telah memilih untuk menambahkan biaya-biaya umum. Hapus semua biaya yang ada sebelum lanjut?", "Konfirmasi Biaya Umum", MessageBoxButton.YesNo);
                if (MessageBoxResult.Yes == answer)
                {
                    data.Costs.Clear();
                }
            }

            List<string> names = (from c in data.Costs select c.Name).ToList();

            if (!names.Contains(COST_LAND_PURCHASE))
            {
                Cost cost1 = new Cost();
                cost1.Name = COST_LAND_PURCHASE;
                cost1.Quantity = data.TotalLandArea;
                cost1.Unit = "m²";
                cost1.UnitValue = data.BaseLandPrice;
                cost1.PropertyChanged += Cost_PropertyChanged;
                data.Costs.Add(cost1);
            }

            if (!names.Contains(COST_ROAD_PURCHASE))
            {
                Cost cost2 = new Cost();
                cost2.Name = COST_ROAD_PURCHASE;
                cost2.Quantity = data.TotalRoadArea;
                cost2.Unit = "m²";
                cost2.UnitValue = data.BaseLandPrice;
                cost2.PropertyChanged += Cost_PropertyChanged;
                data.Costs.Add(cost2);
            }

            Cost cost3 = new Cost();
            cost3.Name = "Drainase";
            cost3.Quantity = 80;
            cost3.Unit = "m";
            cost3.UnitValue = 50000;
            cost3.PropertyChanged += Cost_PropertyChanged;
            data.Costs.Add(cost3);

            Cost cost4 = new Cost();
            cost4.Name = "Resapan";
            cost4.Quantity = 2;
            cost4.Unit = "bh";
            cost4.UnitValue = 1000000;
            cost4.PropertyChanged += Cost_PropertyChanged;
            data.Costs.Add(cost4);

            Cost cost5 = new Cost();
            cost5.Name = "Urug";
            cost5.Quantity = 1;
            cost5.Unit = "ls";
            cost5.UnitValue = 2000000;
            cost5.PropertyChanged += Cost_PropertyChanged;
            data.Costs.Add(cost5);

            Cost cost6 = new Cost();
            cost6.Name = "Pembatas Kavling";
            cost6.Quantity = 68;
            cost6.Unit = "m";
            cost6.UnitValue = 90000;
            cost6.PropertyChanged += Cost_PropertyChanged;
            data.Costs.Add(cost6);

            Cost cost7 = new Cost();
            cost7.Name = "Kontribusi Wilayah";
            cost7.Quantity = data.Lots.Count;
            cost7.Unit = "unit";
            cost7.UnitValue = 1000000;
            cost7.PropertyChanged += Cost_PropertyChanged;
            data.Costs.Add(cost7);

            Cost cost8 = new Cost();
            cost8.Name = "Biaya Pecah";
            cost8.Quantity = data.Lots.Count + 1;
            cost8.Unit = "bh";
            cost8.UnitValue = 2000000;
            cost8.PropertyChanged += Cost_PropertyChanged;
            data.Costs.Add(cost8);

            Cost cost9 = new Cost();
            cost9.Name = "AJB";
            cost9.Quantity = data.Lots.Count;
            cost9.Unit = "bh";
            cost9.UnitValue = 2500000;
            cost9.PropertyChanged += Cost_PropertyChanged;
            data.Costs.Add(cost9);

        }

        private void DeleteCost(object sender, RoutedEventArgs e)
        {
            Cost cost = dgCosts.SelectedItem as Cost;
            if (null != cost)
            {
                // TODO: Use metro-styled MessageBox for theme consistency
                // TODO: Validate to make sure the user doesn't delete mandatory costs
                MessageBoxResult answer = MessageBox.Show(
                    "Apakah Anda yakin ingin menghapus " + cost.Name + "?", "Konfirmasi Menghapus Pekerjaan", MessageBoxButton.OKCancel);
                if (MessageBoxResult.OK == answer)
                {
                    data.Costs.Remove(cost);
                }
            }
            else
            {
                // TODO: Use metro-styled MessageBox for theme consistency
                MessageBox.Show("Silakan pilih dahulu pekerjaan yang akan dihapus.", "Pilih Pekerjaan");
            }
        }

        private void DeleteAllCosts(object sender, RoutedEventArgs e)
        {
            // TODO: Use metro-styled MessageBox for theme consistency
            MessageBoxResult answer = MessageBox.Show(
                    "Apakah Anda yakin ingin menghapus semua pekerjaan?", "Konfirmasi Menghapus Semua Pekerjaan", MessageBoxButton.OKCancel);
            if (MessageBoxResult.OK == answer)
            {
                data.Costs.Clear();
            }
        }

        private void SecondPagePrevious(object sender, RoutedEventArgs e)
        {
            matcPrimaryTabControl.SelectedIndex = 0;
        }

        private void SecondPageNext(object sender, RoutedEventArgs e)
        {
            try
            {
                ValidateAllData();
                RecalculateAllData();
                SetupResultFields();
                matcPrimaryTabControl.SelectedIndex = 2;
            }
            catch (DataValidationException dve)
            {
                MessageBox.Show("Data validation exception occured: " + dve.Message, "Error During Calculation");
            }
        }

        private void ThirdPagePrevious(object sender, RoutedEventArgs e)
        {
            matcPrimaryTabControl.SelectedIndex = 1;
        }

        #endregion

        #endregion

        private decimal RecalculateRemainingLandArea()
        {
            if (null == data.Lots)
            {
                data.Lots = new ObservableCollection<Lot>();
            }

            if (data.TotalLandArea > 0)
            {
                int totalAlottedLandArea = 0;
                foreach (Lot lot in data.Lots)
                {
                    totalAlottedLandArea += lot.LandArea;
                }
                return data.TotalLandArea - data.TotalRoadArea - totalAlottedLandArea;
            }
            else
            {
                return 0;
            }
        }

        private bool ValidateFirstPage()
        {
            bool result = true;

            if (null == data.Lots)
            {
                data.Lots = new ObservableCollection<Lot>();
            }

            if (string.IsNullOrEmpty(data.Location))
            {
                MessageBox.Show("Silakan tentukan nama atau alamat lokasi properti.", "Data Properti Tidak Lengkap");
                txtLocation.Focus();
                return false;
            }

            if (0 == data.TotalLandArea)
            {
                MessageBox.Show("Silakan tentukan luas keseluruhan properti.", "Data Properti Tidak Lengkap");
                txtLandArea.Focus();
                return false;
            }

            if (0 == data.TotalRoadArea)
            {
                MessageBox.Show("Silakan tentukan luas jalan lingkungan yang akan dibuat.", "Data Properti Tidak Lengkap");
                txtRoadArea.Focus();
                return false;
            }

            if (0 == data.BaseLandPrice)
            {
                MessageBox.Show("Silakan tentukan harga dasar tanah per meter.", "Data Properti Tidak Lengkap");
                txtBaseLandPrice.Focus();
                return false;
            }

            if (data.Lots.Count == 0)
            {
                MessageBox.Show("Silakan tambahkan minimal satu kavling.", "Data Kavling Masih Kosong");
                return false;
            }

            else if (RecalculateRemainingLandArea() > 0)
            {
                MessageBox.Show("Masih ada lahan tersisa yang belum dialokasikan ke dalam salah satu kavling. Silakan diperiksa kembali.", "Lahan Tersisa");
                return false;
            }

            return result;
        }

        private void ValidateAllData()
        {
            // verify property variables are complete
            if (string.IsNullOrEmpty(data.Location))
            {
                throw new DataValidationException("Property location not set.");
            }

            if (0 == data.TotalLandArea)
            {
                throw new DataValidationException("Total land area not set.");
            }

            if (0 == data.TotalRoadArea)
            {
                // TODO: This shouldn't be a problem since a block of properties may not have inner roads on its own.
            }

            if (0 == data.BaseLandPrice)
            {
                throw new DataValidationException("Base land price not set.");
            }

            if (0 == data.BuildingPrice)
            {
                throw new DataValidationException("Building price not set.");
            }

            // verify base prices and costs are complete
            if (0 == data.LandResaleProfitPercent)
            {
                // TODO: This shouldn't be a problem since theoretically it's possible to sell the land without profit.
            }

            if (0 == data.BuildingPermitCostPerLot)
            {
                throw new DataValidationException("Building permit cost per lot not set.");
            }

            if (0 == data.PromoCostPerLot)
            {
                // TODO: This shouldn't be a problem since theoretically it's possible to defer promo costs to another budget.
            }

            if (0 == data.FeePercent)
            {
                throw new DataValidationException("Fee percent not set.");
            }

            // verify at least one lot is created
            if (null == data.Lots)
            {
                data.Lots = new ObservableCollection<Lot>();
                throw new DataValidationException("No lots are set.");
            }
            else if (0 == data.Lots.Count)
            {
                throw new DataValidationException("No lots are set.");
            }

            // verify all land area has been allocated
            if (RecalculateRemainingLandArea() > 0)
            {
                throw new DataValidationException("Not all available land area is allocated.");
            }

            // verify all lots have complete initial variables
            foreach (Lot lot in data.Lots)
            {
                if (string.IsNullOrEmpty(lot.Name))
                {
                    throw new DataValidationException("One of the lots' name is not set.");
                }
                if (0 == lot.LandArea)
                {
                    throw new DataValidationException("Land area for lot " + lot.Name + " is not set.");
                }
                if (0 == lot.BuildingArea)
                {
                    // TODO: This shouldn't be a problem since a lot may be sold as land only with no buildings planned.
                }
            }

            // verify at least one cost is created
            if (null == data.Costs)
            {
                InitializeCosts();
            }

            if (0 == data.Costs.Count)
            {
                throw new DataValidationException("No costs have been created for this property.");
            }

            // verify land purchase cost and road purchase cost are created
            List<string> costNames = (from c in data.Costs select c.Name).ToList();

            if (!costNames.Contains(COST_LAND_PURCHASE))
            {
                throw new DataValidationException("No land purchase cost have been created for this property.");
            }

            if (!costNames.Contains(COST_ROAD_PURCHASE))
            {
                throw new DataValidationException("No road purchase cost have been created for this property.");
            }

            // verify all costs have proper unit value, quantity, and total value set
            foreach (Cost cost in data.Costs)
            {
                if (0 == cost.UnitValue)
                {
                    throw new DataValidationException("Unit value not set for cost " + cost.Name);
                }
                if (0 == cost.Quantity)
                {
                    throw new DataValidationException("Quantity not set for cost " + cost.Name);
                }
                if (0 == cost.TotalValue)
                {
                    throw new DataValidationException("Total value not set for cost " + cost.Name);
                }
            }
        }

        private void Cost_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            UpdateSecondPageOnDataChange();
        }

        private void Costs_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            UpdateSecondPageOnDataChange();
        }

        private void UpdateSecondPageOnDataChange()
        {
            RecalculateAllData();
            txtTotalCosts.Text = data.TotalCostsOfDevelopment.ToString("#,###.00");
            txtEffectiveLandCost.Text = data.EffectiveLandCost.ToString("#,###.00");
            txtLandResalePrice.Text = data.LandResalePrice.ToString("#,###.00");
        }

        private void RecalculateAllData()
        {
            // calculate total cost of development = sum of all cost total values
            data.TotalCostsOfDevelopment = 0;
            foreach (Cost cost in data.Costs)
            {
                data.TotalCostsOfDevelopment += cost.TotalValue;
            }

            // calculate effective land cost = total cost of development / (total land area - total road area)
            data.EffectiveLandCost = data.TotalCostsOfDevelopment / (data.TotalLandArea - data.TotalRoadArea);

            // calculate land resale price = effective land cost * (1 + (land resale profit in percent / 100))
            data.LandResalePrice = data.EffectiveLandCost * (1 + (data.LandResaleProfitPercent / 100));

            // for each lot
            data.TotalBaseSalePrice = 0;
            foreach (Lot lot in data.Lots)
            {

                /// set common costs
                lot.BuildingPermitCost = data.BuildingPermitCostPerLot;
                lot.PromoCost = data.PromoCostPerLot;

                //// calculate total building price = building price * building area
                lot.TotalBuildingCost = data.BuildingPrice * lot.BuildingArea;

                //// calculate total land price = land resale price * total land area
                lot.TotalLandCost = data.LandResalePrice * lot.LandArea;

                //// calculate total nett price = total building price
                //// + total land price + building permit + promo cost
                lot.TotalNettPrice = lot.TotalBuildingCost + lot.TotalLandCost
                    + data.BuildingPermitCostPerLot + data.PromoCostPerLot;

                /// calculate value added tax = %VAT * total nett price
                lot.ValueAddedTax = (data.ValueAddedTaxPercent / 100) * lot.TotalNettPrice;

                /// calculate fee = %fee * total nett price
                lot.Fee = (data.FeePercent / 100) * lot.TotalNettPrice;

                /// calculate base sale price = total nett price + VAT + fee
                lot.BaseSalePrice = lot.TotalNettPrice + lot.ValueAddedTax + lot.Fee;

                //// calculate price points
                lot.PricePoints = new ObservableCollection<PricePoint>();
                for (int i = 0; i < data.ProfitPoints.Count(); i++)
                {
                    PricePoint pricePoint = new PricePoint();
                    pricePoint.BaseSalePrice = lot.BaseSalePrice;
                    pricePoint.ProfitAssumptionPercent = data.ProfitPoints[i];
                    pricePoint.ProfitNominal = pricePoint.BaseSalePrice * (pricePoint.ProfitAssumptionPercent / 100);
                    pricePoint.FinalPriceNominal = pricePoint.BaseSalePrice + pricePoint.ProfitNominal;
                    lot.PricePoints.Add(pricePoint);
                }

                //// calculate sum of base sale price
                data.TotalBaseSalePrice += lot.BaseSalePrice;

            }

            // calculate final profit = total base sale price * final profit percentage / 100
            data.FinalProfitNominal = data.TotalBaseSalePrice * (data.FinalProfitPercentage / 100);

            // calculate total actual land value = total land purchase cost + final profit
            data.TotalActualLandValue = 0;
            foreach (Cost cost in data.Costs)
            {
                if (COST_LAND_PURCHASE.Equals(cost.Name))
                {
                    data.TotalActualLandValue = cost.TotalValue + data.FinalProfitNominal;
                    break;
                }
            }

            // calculate actual land value = total actual land value / total land area
            data.ActualLandValue = data.TotalActualLandValue / data.TotalLandArea;

        }

        private void InitializeCosts()
        {
            data.Costs = new ObservableCollection<Cost>();
            data.Costs.CollectionChanged += Costs_CollectionChanged;
        }

        private void SetupResultFields()
        {
            int lotBasedDataGridHeight = 28 * (data.Lots.Count() + 1);
            int costBasedDataGridHeight = 28 * (data.Costs.Count() + 1);

            lblResultLocation.Content = data.Location;
            lblResultLandArea.Content = data.TotalLandArea;
            lblResultRoadArea.Content = data.TotalRoadArea;
            lblResultBaseLandPrice.Content = data.BaseLandPrice;

            dgResultBasicLots.Height = lotBasedDataGridHeight;
            dgResultCosts.Height = costBasedDataGridHeight;
            
            lblResultTotalCostsOfDevelopment.Content = data.TotalCostsOfDevelopment;
            lblResultEffectiveLandCost.Content = data.EffectiveLandCost;
            lblResultLandResaleProfitPercent.Content = data.LandResaleProfitPercent;
            lblResultLandResalePrice.Content = data.LandResalePrice;
            lblResultBuildingPrice.Content = data.BuildingPrice;

            dgResultLotNettPrice.Height = lotBasedDataGridHeight;

            dgResultLotBaseSalePrice.Columns[1].Header = "PPH " + data.ValueAddedTaxPercent + "%";
            dgResultLotBaseSalePrice.Columns[2].Header = "Fee " + data.FeePercent + "%";
            dgResultLotBaseSalePrice.Height = lotBasedDataGridHeight;

            dgResultLotSalePrice.Columns[3].Header = "HJ Profit " + data.ProfitPoints[0] + "%";
            dgResultLotSalePrice.Columns[4].Header = "HJ Profit " + data.ProfitPoints[1] + "%";
            dgResultLotSalePrice.Columns[5].Header = "HJ Profit " + data.ProfitPoints[2] + "%";
            dgResultLotSalePrice.Columns[6].Header = "HJ Profit " + data.ProfitPoints[3] + "%";
            dgResultLotSalePrice.Height = lotBasedDataGridHeight;

            dgResultLotProfit.Columns[3].Header = "Profit " + data.ProfitPoints[0] + "%";
            dgResultLotProfit.Columns[4].Header = "Profit " + data.ProfitPoints[1] + "%";
            dgResultLotProfit.Columns[5].Header = "Profit " + data.ProfitPoints[2] + "%";
            dgResultLotProfit.Columns[6].Header = "Profit " + data.ProfitPoints[3] + "%";
            dgResultLotProfit.Height = lotBasedDataGridHeight;

            lblResultLandPriceActual10.Content = "Profit 10%";
            lblResultLandPriceActualP1.Content = "Profit " + data.ProfitPoints[0] + "%";
            lblResultLandPriceActualP2.Content = "Profit " + data.ProfitPoints[1] + "%";
            lblResultLandPriceActualP3.Content = "Profit " + data.ProfitPoints[2] + "%";
            lblResultLandPriceActualP4.Content = "Profit " + data.ProfitPoints[3] + "%";
            lblResultLandPriceNominal10.Content = data.ActualLandValue;
            lblResultLandPriceNominalP1.Content = (((data.ProfitPoints[0] / 10) * data.FinalProfitNominal) + (data.TotalLandArea * data.BaseLandPrice)) / data.TotalLandArea;
            lblResultLandPriceNominalP2.Content = (((data.ProfitPoints[1] / 10) * data.FinalProfitNominal) + (data.TotalLandArea * data.BaseLandPrice)) / data.TotalLandArea;
            lblResultLandPriceNominalP3.Content = (((data.ProfitPoints[2] / 10) * data.FinalProfitNominal) + (data.TotalLandArea * data.BaseLandPrice)) / data.TotalLandArea;
            lblResultLandPriceNominalP4.Content = (((data.ProfitPoints[3] / 10) * data.FinalProfitNominal) + (data.TotalLandArea * data.BaseLandPrice)) / data.TotalLandArea;
        }

    }
}
