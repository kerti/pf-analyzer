using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using pf_analyzer.Common;
using pf_analyzer.DataModel;
using pf_analyzer.Exceptions;
using pf_analyzer.Extensions;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace pf_analyzer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {

        #region Private Properties

        private PropertyDataModel data;

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

            data = new PropertyDataModel();
            data.Initialize();
            this.DataContext = data;
            data.RecalculateRemainingLandArea();
        }

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

        private void ShowAboutPage(object sender, RoutedEventArgs e)
        {
            About about = new About();
            about.Owner = this;
            about.ShowDialog();
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
            lblRemainingAreaNominal.Content = data.RecalculateRemainingLandArea().ToString();
        }

        private async void DeleteLot(object sender, RoutedEventArgs e)
        {
            System.Collections.IList selectedLots = dgBasicLots.SelectedItems;
            if (null != selectedLots && selectedLots.Count > 0)
            {
                MessageDialogResult answer = await this.ShowMessageAsync(
                    "Konfirmasi Menghapus Kavling",
                    "Apakah Anda yakin ingin menghapus kavling-kavling yang telah Anda tandai?"
                        + "\n\n Silakan tekan \"tidak\" untuk memeriksa kembali."
                        + "\n Silakan tekan \"ya\" untuk melanjutkan penghapusan.",
                    MessageDialogStyle.AffirmativeAndNegative,
                    Constants.MDS_YESNO);
                if (MessageDialogResult.Affirmative == answer)
                {
                    List<Lot> lots = new List<Lot>();
                    for (int i = 0; i < selectedLots.Count; i++)
                    {
                        if (selectedLots[i] is Lot)
                        {
                            Lot lot = selectedLots[i] as Lot;
                            lots.Add(lot);
                        }
                    }
                    foreach (Lot lot in lots)
                    {
                        data.Lots.Remove(lot);
                    }
                }
            }
            else
            {
                await this.ShowMessageAsync(
                    "Pilih Kavling",
                    "Silakan pilih dahulu kavling yang akan dihapus.",
                    MessageDialogStyle.Affirmative,
                    Constants.MDS_OKAY);
            }
        }

        private async void FirstPageNext(object sender, RoutedEventArgs e)
        {
            if (await ValidateFirstPage())
            {
                matcPrimaryTabControl.SelectedIndex = 1;
            }
        }

        #endregion

        #region Page Two

        private async void AddDefaultCosts(object sender, RoutedEventArgs e)
        {
            bool clearBeforeAdd = false;

            if (data.Costs.Count > 0)
            {
                MessageDialogResult answer = await this.ShowMessageAsync(
                    "Konfirmasi Biaya Umum",
                    "Anda telah memilih untuk menambahkan biaya-biaya umum. Hapus semua biaya yang ada sebelum lanjut?",
                    MessageDialogStyle.AffirmativeAndNegative,
                    Constants.MDS_YESNO);
                clearBeforeAdd = MessageDialogResult.Affirmative == answer;
            }

            data.AddDefaultCosts(clearBeforeAdd, Costs_CollectionChanged, Cost_PropertyChanged); 
        }

        private async void DeleteCost(object sender, RoutedEventArgs e)
        {
            System.Collections.IList selectedCosts = dgCosts.SelectedItems;
            if (null != selectedCosts && selectedCosts.Count > 0)
            {
                // TODO: Validate to make sure the user doesn't delete mandatory costs
                MessageDialogResult answer = await this.ShowMessageAsync(
                    "Konfirmasi Menghapus Pekerjaan",
                    "Apakah Anda yakin ingin menghapus pekerjaan-pekerjaan yang telah Anda tandai?"
                        + "\n\n Silakan tekan \"tidak\" untuk memeriksa kembali."
                        + "\n Silakan tekan \"ya\" untuk melanjutkan penghapusan.",
                    MessageDialogStyle.AffirmativeAndNegative,
                    Constants.MDS_YESNO);
                if (MessageDialogResult.Affirmative == answer)
                {
                    List<Cost> costs = new List<Cost>();
                    for (int i = 0; i < selectedCosts.Count; i++)
                    {
                        if (selectedCosts[i] is Cost)
                        {
                            Cost cost = selectedCosts[i] as Cost;
                            costs.Add(cost);
                        }
                    }
                    foreach (Cost cost in costs)
                    {
                        data.Costs.Remove(cost);
                    }
                }
            }
            else
            {
                await this.ShowMessageAsync(
                    "Pilih Kavling",
                    "Silakan pilih dahulu kavling yang akan dihapus.",
                    MessageDialogStyle.Affirmative,
                    Constants.MDS_OKAY);
            }
        }

        private async void DeleteAllCosts(object sender, RoutedEventArgs e)
        {
            MessageDialogResult answer = await this.ShowMessageAsync(
                    "Konfirmasi Menghapus Semua Pekerjaan",
                    "Apakah Anda yakin ingin menghapus semua pekerjaan?",
                    MessageDialogStyle.AffirmativeAndNegative,
                    Constants.MDS_YESNO);
            if (MessageDialogResult.Affirmative == answer)
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
                data.Validate();
                data.Calculate();
                SetupResultFields();
                matcPrimaryTabControl.SelectedIndex = 2;
            }
            catch (DataValidationException dve)
            {
                this.ShowMessageAsync(
                    "Mohon Periksa Kembali Data", dve.Message,
                    MessageDialogStyle.Affirmative, Constants.MDS_OKAY);
            }
        }

        private void ThirdPagePrevious(object sender, RoutedEventArgs e)
        {
            matcPrimaryTabControl.SelectedIndex = 1;
        }

        #endregion

        #endregion

        private async Task<bool> ValidateFirstPage()
        {
            bool result = true;

            if (null == data.Lots)
            {
                data.Lots = new ObservableCollection<Lot>();
            }

            if (string.IsNullOrEmpty(data.Location))
            {
                await this.ShowMessageAsync(
                    "Data Properti Tidak Lengkap", "Silakan tentukan nama atau alamat lokasi properti.",
                    MessageDialogStyle.Affirmative, Constants.MDS_OKAY);
                txtLocation.Focus();
                return false;
            }

            if (0 == data.TotalLandArea)
            {
                await this.ShowMessageAsync(
                    "Data Properti Tidak Lengkap", "Silakan tentukan luas keseluruhan properti.",
                    MessageDialogStyle.Affirmative, Constants.MDS_OKAY);
                txtLandArea.Focus();
                return false;
            }

            if (data.TotalLandArea <= (data.TotalRoadArea + data.TotalPublicFacilityArea))
            {
                await this.ShowMessageAsync(
                    "Mohon Periksa Kembali Data", "Silakan periksa kembali luas jalan dan fasum, "
                        + "seharusnya jumlah keduanya masih menyisakan lahan untuk kavling.",
                    MessageDialogStyle.Affirmative, Constants.MDS_OKAY);
                txtRoadArea.Focus();
                return false;
            }

            if (0 == data.BaseLandPrice)
            {
                await this.ShowMessageAsync(
                    "Data Properti Tidak Lengkap", "Silakan tentukan harga dasar tanah per meter.",
                    MessageDialogStyle.Affirmative, Constants.MDS_OKAY);
                txtBaseLandPrice.Focus();
                return false;
            }

            if (data.Lots.Count == 0)
            {
                await this.ShowMessageAsync(
                    "Data Kavling Masih Kosong", "Silakan tambahkan minimal satu kavling.",
                    MessageDialogStyle.Affirmative, Constants.MDS_OKAY);
                return false;
            }
            else
            {
                decimal remainingLandArea = data.RecalculateRemainingLandArea();
                if (remainingLandArea > 0)
                {
                    await this.ShowMessageAsync(
                        "Lahan Tersisa", "Masih ada lahan tersisa yang belum dialokasikan ke dalam salah satu kavling. Silakan diperiksa kembali.",
                        MessageDialogStyle.Affirmative, Constants.MDS_OKAY);
                    return false;
                }
                else if (remainingLandArea < 0)
                {
                    await this.ShowMessageAsync(
                        "Lahan Tersisa", "Lahan yang dialokasikan untuk kavling telah melebihi lahan yang tersisa. Silakan diperiksa kembali.",
                        MessageDialogStyle.Affirmative, Constants.MDS_OKAY);
                    return false;
                }
            }

            return result;
        }

        private void Cost_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            UpdateSecondPageOnDataChange();
        }

        private void Costs_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            UpdateSecondPageOnDataChange();
        }

        private void UpdateSecondPageOnDataChange()
        {
            data.Calculate();
            txtTotalCosts.Text = data.TotalCostsOfDevelopment.ToString("#,###.00");
            txtEffectiveLandCost.Text = data.EffectiveLandCost.ToString("#,###.00");
            txtLandResalePrice.Text = data.LandResalePrice.ToString("#,###.00");
        }

        private void SetupResultFields()
        {
            int lotBasedDataGridHeight = 28 * (data.Lots.Count() + 1);
            int costBasedDataGridHeight = 28 * (data.Costs.Count() + 1);

            lblResultLocation.Content = data.Location;
            lblResultLandArea.Content = data.TotalLandArea;
            lblResultRoadArea.Content = data.TotalRoadArea;
            lblResultPublicFacilityArea.Content = data.TotalPublicFacilityArea;
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

            // refresh datagrids
            dgResultBasicLots.Items.Refresh();
            dgResultCosts.Items.Refresh();
            dgResultLotBaseSalePrice.Items.Refresh();
            dgResultLotNettPrice.Items.Refresh();
            dgResultLotProfit.Items.Refresh();
            dgResultLotSalePrice.Items.Refresh();
        }

    }
}