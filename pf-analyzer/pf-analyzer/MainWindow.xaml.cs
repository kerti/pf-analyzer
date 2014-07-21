using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Win32;
using pf_analyzer.Common;
using pf_analyzer.DataModel;
using pf_analyzer.Exceptions;
using pf_analyzer.Extensions;

namespace pf_analyzer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {

        #region Private Properties

        private PropertyDataModel data;
        private string filename = string.Empty;
        private bool canOpenFile = true;

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

        #endregion Common Control Events

        #region Window Drag and Drop Events

        private void Window_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent("Filename"))
            {
                MemoryStream ms = e.Data.GetData("Filename") as MemoryStream;
                StreamReader sr = new StreamReader(ms);
                string info = sr.ReadToEnd();
                if (Constants.FILE_EXTENSION.Equals(info.Substring(info.Length - 5, 4)))
                {
                    canOpenFile = true;
                }
                else
                {
                    canOpenFile = false;
                }
            }
            else
            {
                canOpenFile = false;
            }
            e.Handled = true;
        }

        private void Window_DragOver(object sender, DragEventArgs e)
        {
            if (canOpenFile)
            {
                e.Effects = DragDropEffects.All;
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }
            e.Handled = true;
        }

        private void Window_DragLeave(object sender, DragEventArgs e)
        {
            canOpenFile = true;
            e.Effects = DragDropEffects.None;
            e.Handled = true;
        }

        private void Window_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent("Filename"))
            {
                MemoryStream ms = e.Data.GetData("Filename") as MemoryStream;
                StreamReader sr = new StreamReader(ms);
                string info = sr.ReadToEnd();
                ReadXmlDocument(info.Substring(0, info.Length - 1));
            }
        }

        #endregion Window Drag and Drop Events

        #region Commands

        #region Open File Command

        private void CanOpenFile(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = canOpenFile;
            e.Handled = true;
        }

        private void OpenFile(object sender, ExecutedRoutedEventArgs e)
        {
            OpenFileDialog ofdOpenFile = new OpenFileDialog();

            ofdOpenFile.Filter = Constants.FILE_TYPE
                + " (*" + Constants.FILE_EXTENSION + ")|*"
                + Constants.FILE_EXTENSION + "|All files (*.*)|*.*";
            ofdOpenFile.FilterIndex = 1;
            ofdOpenFile.RestoreDirectory = true;

            if (ofdOpenFile.ShowDialog().Value)
            {
                ReadXmlDocument(ofdOpenFile.FileName);
            }
        }

        #endregion Open File Command

        #region Save File Command

        private void CanSaveFile(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
            e.Handled = true;
        }

        private void SaveFile(object sender, ExecutedRoutedEventArgs e)
        {
            bool isSaveAs = true;
            if (!string.IsNullOrEmpty(filename))
            {
                isSaveAs = false;
            }
            else
            {
                SaveFileDialog sfdSaveFile = new SaveFileDialog();

                sfdSaveFile.Filter = Constants.FILE_TYPE
                    + " (*" + Constants.FILE_EXTENSION + ")|*"
                    + Constants.FILE_EXTENSION + "|All files (*.*)|*.*";
                sfdSaveFile.FilterIndex = 1;
                sfdSaveFile.RestoreDirectory = true;

                if (sfdSaveFile.ShowDialog().Value)
                {
                    filename = sfdSaveFile.FileName;
                }
            }

            try
            {
                if (!string.IsNullOrEmpty(filename))
                {
                    using (StreamWriter sw = new StreamWriter(filename))
                    {
                        string xmlString = data.ToXML();
                        sw.Write(xmlString);
                    }

                    if (isSaveAs)
                    {
                        ShowMessageNoWait(
                            "Berhasil Menyimpan File",
                            "Berhasil menyimpan file sebagai " + filename + ".");
                    }
                }
                else
                {
                    ShowMessageNoWait(
                        "Gagal Menyimpan File",
                        "Gagal menyimpan file karena nama file tidak ditentukan.");
                }
            }
            catch (InvalidOperationException)
            {
                ShowMessageNoWait(
                    "Gagal Menyimpan File",
                    "Gagal menyimpan file karena tidak dapat mengenali format file tersebut.");
            }
            catch (Exception ex)
            {
                ShowMessageNoWait(
                    "Gagal Menyimpan File",
                    "Gagal menyimpan file karena kesalahan berikut: " + ex.Message);
            }
        }

        #endregion Save File Command

        #endregion Commands

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

        private async void ShowMessageNoWait(string title, string message,
            MessageDialogStyle style = MessageDialogStyle.Affirmative,
            MetroDialogSettings settings = null)
        {
            if (null == settings)
            {
                settings = Constants.MDS_OKAY;
            }
            await this.ShowMessageAsync(title, message, style, settings);
        }

        private void ReadXmlDocument(string attemptedFilename)
        {
            try
            {
                if (!string.IsNullOrEmpty(attemptedFilename))
                {
                    using (StreamReader sr = File.OpenText(attemptedFilename))
                    {
                        string s = sr.ReadToEnd();
                        CopyLoadedData(data.FromXML(s));
                        LoadXmlDataToScreen();
                        filename = attemptedFilename;
                    }
                }
                else
                {
                    ShowMessageNoWait(
                        "Gagal Membuka File",
                        "Gagal membuka file karena nama file tidak ditentukan..");
                }
            }
            catch (InvalidOperationException)
            {
                ShowMessageNoWait(
                    "Gagal Membuka File",
                    "Gagal membuka file karena tidak dapat mengenali format file tersebut.");
            }
            catch (Exception ex)
            {
                ShowMessageNoWait(
                    "Gagal Membuka File",
                    "Gagal membuka file karena kesalahan berikut: " + ex.Message);
            }
        }

        private void CopyLoadedData(PropertyDataModel converted)
        {

            #region Page One

            data.Location = converted.Location;
            data.TotalLandArea = converted.TotalLandArea;
            data.TotalRoadArea = converted.TotalRoadArea;
            data.TotalPublicFacilityArea = converted.TotalPublicFacilityArea;
            data.BaseLandPrice = converted.BaseLandPrice;

            if (null != converted.Lots && converted.Lots.Count > 0)
            {
                foreach (Lot lot in converted.Lots)
                {
                    data.Lots.Add(lot);
                }
            }

            #endregion

            #region Page Two

            if (null != converted.Costs && converted.Costs.Count > 0)
            {
                foreach (Cost cost in converted.Costs)
                {
                    cost.PropertyChanged += Cost_PropertyChanged;
                    data.Costs.Add(cost);
                }
            }

            data.TotalCostsOfDevelopment = converted.TotalCostsOfDevelopment;
            data.EffectiveLandCost = converted.EffectiveLandCost;
            data.LandResaleProfitPercent = converted.LandResaleProfitPercent;
            data.LandResalePrice = converted.LandResalePrice;
            data.BuildingPrice = converted.BuildingPrice;
            data.BuildingPermitCostPerLot = converted.BuildingPermitCostPerLot;
            data.PromoCostPerLot = converted.PromoCostPerLot;
            data.ValueAddedTaxPercent = converted.ValueAddedTaxPercent;
            data.FeePercent = converted.FeePercent;
            data.ProfitPoints = converted.ProfitPoints;
            data.TotalBaseSalePrice = converted.TotalBaseSalePrice;
            data.FinalProfitPercentage = converted.FinalProfitPercentage;
            data.FinalProfitNominal = converted.FinalProfitNominal;
            data.TotalActualLandValue = converted.TotalActualLandValue;
            data.ActualLandValue = converted.ActualLandValue;

            #endregion

        }

        private void LoadXmlDataToScreen()
        {
            if (null != data)
            {

                #region Page One

                if (!string.IsNullOrEmpty(data.Location))
                {
                    txtLocation.Text = data.Location;
                }

                txtLandArea.Text = data.TotalLandArea.ToString("#,##0.00");
                txtRoadArea.Text = data.TotalRoadArea.ToString("#,##0.00");
                txtPublicFacilityArea.Text = data.TotalPublicFacilityArea.ToString("#,##0.00");
                txtBaseLandPrice.Text = data.BaseLandPrice.ToString("#,##0.00");

                #endregion Page One

                #region Page Two

                txtTotalCosts.Text = data.TotalCostsOfDevelopment.ToString("#,##0.00");
                txtEffectiveLandCost.Text = data.EffectiveLandCost.ToString("#,##0.00");
                txtLandResaleProfitPercent.Text = data.LandResaleProfitPercent.ToString("#,##0.00");
                txtLandResalePrice.Text = data.LandResalePrice.ToString("#,##0.00");
                txtBuildingPrice.Text = data.BuildingPrice.ToString("#,##0.00");
                txtBuildingPermitCostPerLot.Text = data.BuildingPermitCostPerLot.ToString("#,##0.00");
                txtPromoCostPerLot.Text = data.PromoCostPerLot.ToString("#,##0.00");
                txtValueAddedTaxPercent.Text = data.ValueAddedTaxPercent.ToString("#,##0.00");
                txtFeePercent.Text = data.FeePercent.ToString("#,##0.00");

                #endregion Page Two

            }
        }

    }
}