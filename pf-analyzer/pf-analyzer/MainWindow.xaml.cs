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
using PFAnalyzer.Common;
using PFAnalyzer.DataModel;
using PFAnalyzer.Exceptions;
using PFAnalyzer.Extensions;

namespace PFAnalyzer
{
    /// <summary>
    /// Interaction logic for Main Window.
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        #region Private Properties

        /// <summary>
        /// The <see cref="DataModel.PropertyDataModel"/> object that is to be used throughout the screen.
        /// </summary>
        private PropertyDataModel data;

        /// <summary>
        /// The filename of the currently open file in the application. Should be empty when the file is closed.
        /// </summary>
        private string filename = string.Empty;

        /// <summary>
        /// A boolean indicating whether or not the application is in a state that allows for opening a new file.
        /// </summary>
        private bool canOpenFile = true;

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
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

            // Initialise the data model
            data = new PropertyDataModel();
            data.Initialize();
            this.DataContext = data;
            data.RecalculateRemainingLandArea();
        }

        #region Control Events

        #region Common Control Events

        /// <summary>
        /// Event handler for Textbox Got Keyboard Focus event.
        /// </summary>
        /// <remarks>
        /// This event handler will select all text on the Textbox.
        /// </remarks>
        /// <param name="sender">
        /// The sender of the event.
        /// </param>
        /// <param name="e">
        /// The parameters of the event.
        /// </param>
        private void TextboxGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            (e.OriginalSource as TextBox).SelectAll();
        }

        /// <summary>
        /// Event handler for Textbox Got Mouse Capture event.
        /// </summary>
        /// <remarks>
        /// This event handler will select all text on the Textbox.
        /// </remarks>
        /// <param name="sender">
        /// The sender of the event.
        /// </param>
        /// <param name="e">
        /// The parameters of the event.
        /// </param>
        private void TextboxGotMouseCapture(object sender, MouseEventArgs e)
        {
            (e.OriginalSource as TextBox).SelectAll();
        }

        /// <summary>
        /// Event handler for Button Click event.
        /// </summary>
        /// <remarks>
        /// This event handler will display the About page.
        /// </remarks>
        /// <param name="sender">
        /// The sender of the event.
        /// </param>
        /// <param name="e">
        /// The parameters of the event.
        /// </param>
        private void ShowAboutPage(object sender, RoutedEventArgs e)
        {
            About about = new About();
            about.Owner = this;
            about.ShowDialog();
        }

        /// <summary>
        /// Event handler for Textbox Preview Text Input event.
        /// </summary>
        /// <remarks>
        /// This event handler will prevent non-numeric character inputs.
        /// </remarks>
        /// <param name="sender">
        /// The sender of the event.
        /// </param>
        /// <param name="e">
        /// The parameters of the event.
        /// </param>
        private void TextboxPreviewTextInputNumericOnly(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !char.IsNumber(Convert.ToChar(e.Text));
            OnPreviewTextInput(e);
        }

        /// <summary>
        /// Event handler for Data Grid Preview Mouse Wheel event.
        /// </summary>
        /// <remarks>
        /// This event handler will defer mouse wheel scroll events to parent control elements on
        /// Data Grids that use it.
        /// </remarks>
        /// <param name="sender">
        /// The sender of the event.
        /// </param>
        /// <param name="e">
        /// The parameters of the event.
        /// </param>
        private void DeferScrollToParent(object sender, MouseWheelEventArgs e)
        {
            if (!e.Handled)
            {
                e.Handled = true;
                var eventArg = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta);
                eventArg.RoutedEvent = UIElement.MouseWheelEvent;
                eventArg.Source = sender;
                var parent = ((Control)sender).Parent as UIElement;
                parent.RaiseEvent(eventArg);
            }
        }

        /// <summary>
        /// Event handler for Data Grid Mouse Preview event.
        /// </summary>
        /// <remarks>
        /// This event handler enables the Data Grid to perform single-click edits.
        /// </remarks>
        /// <param name="sender">
        /// The sender of the event.
        /// </param>
        /// <param name="e">
        /// The parameters of the event.
        /// </param>
        private void DataGridMousePreviewSingleClickEdit(object sender, RoutedEventArgs e)
        {
            // Lookup for the source to be DataGridCell
            if (e.OriginalSource.GetType() == typeof(DataGridCell))
            {
                // Starts the Edit on the row;
                DataGrid grd = (DataGrid)sender;
                grd.BeginEdit(e);

                Control control = GetFirstChildByType<Control>(e.OriginalSource as DataGridCell);
                if (control != null)
                {
                    control.Focus();
                }
            }
        }

        /// <summary>
        /// Fetches the first child of a control element.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the control to be fetched.
        /// </typeparam>
        /// <param name="prop">
        /// The control containing the element to be fetched.
        /// </param>
        /// <returns>
        /// The fetched control.
        /// </returns>
        private T GetFirstChildByType<T>(DependencyObject prop) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(prop); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(prop, i) as DependencyObject;
                if (null == child)
                {
                    continue;
                }

                T castedProp = child as T;
                if (castedProp != null)
                {
                    return castedProp;
                }

                castedProp = GetFirstChildByType<T>(child);

                if (castedProp != null)
                {
                    return castedProp;
                }
            }

            return null;
        }

        #endregion Common Control Events

        #region Window Drag and Drop Events

        /// <summary>
        /// Event handler for drag-and-drop operations, specifically when the mouse enters the screen.
        /// </summary>
        /// <param name="sender">
        /// The sender of the event.
        /// </param>
        /// <param name="e">
        /// The parameter of the event.
        /// </param>
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

        /// <summary>
        /// Event handler for drag-and-drop operations, specifically when the mouse moves about on the screen.
        /// </summary>
        /// <param name="sender">
        /// The sender of the event.
        /// </param>
        /// <param name="e">
        /// The parameter of the event.
        /// </param>
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

        /// <summary>
        /// Event handler for drag-and-drop operations, specifically when the mouse leaves the screen.
        /// </summary>
        /// <param name="sender">
        /// The sender of the event.
        /// </param>
        /// <param name="e">
        /// The parameter of the event.
        /// </param>
        private void Window_DragLeave(object sender, DragEventArgs e)
        {
            canOpenFile = true;
            e.Effects = DragDropEffects.None;
            e.Handled = true;
        }

        /// <summary>
        /// Event handler for drag-and-drop operations, specifically when a drop is performed.
        /// </summary>
        /// <param name="sender">
        /// The sender of the event.
        /// </param>
        /// <param name="e">
        /// The parameter of the event.
        /// </param>
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

        /// <summary>
        /// Event handler to determine whether an "Open" command is available.
        /// </summary>
        /// <param name="sender">
        /// The sender of the event.
        /// </param>
        /// <param name="e">
        /// The parameter of the event.
        /// </param>
        private void CanOpenFile(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = canOpenFile;
            e.Handled = true;
        }

        /// <summary>
        /// Event handler for executing the "Open" command.
        /// </summary>
        /// <remarks>
        /// The command will prompt the user for a file to open, and then loads the contents of the file
        /// to both the underlying <see cref="DataModel.PropertyDataModel"/> and corresponding fields on
        /// the UI.
        /// </remarks>
        /// <param name="sender">
        /// The sender of the event.
        /// </param>
        /// <param name="e">
        /// The parameter of the event.
        /// </param>
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

        /// <summary>
        /// Event handler to determine whether a "Save" command is available.
        /// </summary>
        /// <param name="sender">
        /// The sender of the event.
        /// </param>
        /// <param name="e">
        /// The parameter of the event.
        /// </param>
        private void CanSaveFile(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
            e.Handled = true;
        }

        /// <summary>
        /// Event handler for executing the "Save" command.
        /// </summary>
        /// <remarks>
        /// The command checks whether a filename is stored, and then prompts the user for the name of the
        /// file if one does not exist. After that, it converts the underlying <see cref="DataModel.PropertyDataModel"/>
        /// object into an XML string and writes it to the selected file.
        /// </remarks>
        /// <param name="sender">
        /// The sender of the event.
        /// </param>
        /// <param name="e">
        /// The parameter of the event.
        /// </param>
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

        #region New File Command

        /// <summary>
        /// Event handler to determine whether a "New" command is available.
        /// </summary>
        /// <param name="sender">
        /// The sender of the event.
        /// </param>
        /// <param name="e">
        /// The parameter of the event.
        /// </param>
        private void CanCreateNewFile(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
            e.Handled = true;
        }

        /// <summary>
        /// Event handler for executing the "New" command.
        /// </summary>
        /// <remarks>
        /// The command reinitializes the underlying <see cref="DataModel.PropertyDataModel"/>,
        /// resets the UI fields in the process, and empties the stored filename string.
        /// </remarks>
        /// <param name="sender">
        /// The sender of the event.
        /// </param>
        /// <param name="e">
        /// The parameter of the event.
        /// </param>
        private void CreateNewFile(object sender, ExecutedRoutedEventArgs e)
        {
            // Initialise the data model
            data = new PropertyDataModel();
            data.Initialize();
            this.DataContext = data;
            data.RecalculateRemainingLandArea();
            filename = string.Empty;
        }

        #endregion New File Command

        #endregion Commands

        #region Page One

        /// <summary>
        /// Event handler for setting the remaining land area label.
        /// </summary>
        /// <param name="sender">
        /// The sender of the event.
        /// </param>
        /// <param name="e">
        /// The parameters of the event.
        /// </param>
        private void HandleSetRemainingLandAreaLabel(object sender, DataGridCellEditEndingEventArgs e)
        {
            SetRemainingLandAreaLabel();
        }

        /// <summary>
        /// Event handler for setting the remaining land area label.
        /// </summary>
        /// <param name="sender">
        /// The sender of the event.
        /// </param>
        /// <param name="e">
        /// The parameters of the event.
        /// </param>
        private void HandleSetRemainingLandAreaLabel(object sender, RoutedEventArgs e)
        {
            SetRemainingLandAreaLabel();
        }

        /// <summary>
        /// Set the remaining land area label based on changes made to the lots collection.
        /// </summary>
        private void SetRemainingLandAreaLabel()
        {
            lblRemainingAreaNominal.Content = data.RecalculateRemainingLandArea().ToString();
        }

        /// <summary>
        /// Event handler for when the user wants to delete a lot.
        /// </summary>
        /// <remarks>
        /// Before initiating the delete, the event handler checks whether or not a lot is actually selected
        /// and displays the appropriate message if a lot is not selected. If a lot is selected, the event will
        /// display a confirmation message before proceeding to delete.
        /// </remarks>
        /// <param name="sender">
        /// The sender of the event.
        /// </param>
        /// <param name="e">
        /// The parameters of the event.
        /// </param>
        private async void DeleteLot(object sender, RoutedEventArgs e)
        {
            System.Collections.IList selectedLots = dgBasicLots.SelectedItems;
            if (null != selectedLots && selectedLots.Count > 0)
            {
                string message = "Apakah Anda yakin ingin menghapus kavling-kavling yang telah Anda tandai?"
                        + "\n\n Silakan tekan \"tidak\" untuk memeriksa kembali."
                        + "\n Silakan tekan \"ya\" untuk melanjutkan penghapusan.";
                MessageDialogResult answer = await this.ShowMessageAsync(
                    "Konfirmasi Menghapus Kavling",
                    message,
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

        /// <summary>
        /// Event handler for when the user wants to advance to second page.
        /// </summary>
        /// <remarks>
        /// When advancing to the second page, first page data is validated. If the data is valid, data on the
        /// second page is updated. If the data is not valid, page change is prevented.
        /// </remarks>
        /// <param name="sender">
        /// The sender of the event.
        /// </param>
        /// <param name="e">
        /// The parameters of the event.
        /// </param>
        private async void FirstPageNext(object sender, RoutedEventArgs e)
        {
            if (await ValidateFirstPage())
            {
                UpdateMandatoryCosts();
                UpdateSecondPageOnDataChange();
                matcPrimaryTabControl.SelectedIndex = 1;
            }
        }

        #endregion

        #region Page Two

        /// <summary>
        /// Event handler for when the user wants to add preset default costs to the analysis.
        /// </summary>
        /// <remarks>
        /// Before performing the actual add, the user will be asked if existing costs should be deleted
        /// before adding default costs.
        /// </remarks>
        /// <param name="sender">
        /// The sender of the event.
        /// </param>
        /// <param name="e">
        /// The parameters of the event.
        /// </param>
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

        /// <summary>
        /// Event handler for when the user wants to delete a costs.
        /// </summary>
        /// <remarks>
        /// Before performing the actual delete, a confirmation message will be displayed.
        /// </remarks>
        /// <param name="sender">
        /// The sender of the event.
        /// </param>
        /// <param name="e">
        /// The parameters of the event.
        /// </param>
        private async void DeleteCost(object sender, RoutedEventArgs e)
        {
            System.Collections.IList selectedCosts = dgCosts.SelectedItems;
            if (null != selectedCosts && selectedCosts.Count > 0)
            {
                string message = "Apakah Anda yakin ingin menghapus pekerjaan-pekerjaan yang telah Anda tandai?"
                        + "\n\n Silakan tekan \"tidak\" untuk memeriksa kembali."
                        + "\n Silakan tekan \"ya\" untuk melanjutkan penghapusan.";

                // TODO: Validate to make sure the user doesn't delete mandatory costs
                MessageDialogResult answer = await this.ShowMessageAsync(
                    "Konfirmasi Menghapus Pekerjaan",
                    message,
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

        /// <summary>
        /// Event handler for when the user wants to delete all costs.
        /// </summary>
        /// <remarks>
        /// Before performing the actual delete, a confirmation message will be displayed.
        /// </remarks>
        /// <param name="sender">
        /// The sender of the event.
        /// </param>
        /// <param name="e">
        /// The parameters of the event.
        /// </param>
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

        /// <summary>
        /// Event handler for when the user wants to go back to the second page.
        /// </summary>
        /// <param name="sender">
        /// The sender of the event.
        /// </param>
        /// <param name="e">
        /// The parameters of the event.
        /// </param>
        private void SecondPagePrevious(object sender, RoutedEventArgs e)
        {
            matcPrimaryTabControl.SelectedIndex = 0;
        }

        /// <summary>
        /// Event handler for when the user wants to advance to the third page.
        /// </summary>
        /// <remarks>
        /// When advancing to third page, the data is revalidated and recalculated, and all fields
        /// on the third page are reset to display the correct calculation results.
        /// </remarks>
        /// <param name="sender">
        /// The sender of the event.
        /// </param>
        /// <param name="e">
        /// The parameters of the event.
        /// </param>
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

        /// <summary>
        /// Event handler for when the user wants to go back to the second page.
        /// </summary>
        /// <param name="sender">
        /// The sender of the event.
        /// </param>
        /// <param name="e">
        /// The parameters of the event.
        /// </param>
        private void ThirdPagePrevious(object sender, RoutedEventArgs e)
        {
            matcPrimaryTabControl.SelectedIndex = 1;
        }

        #endregion

        #endregion

        /// <summary>
        /// Validates the first page and displays a validation message when the data does not pass validation.
        /// </summary>
        /// <returns>
        /// Boolean value indicating whether the first page data passes validation or not.
        /// </returns>
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
                string message = "Silakan periksa kembali luas jalan dan fasum, "
                        + "seharusnya jumlah keduanya masih menyisakan lahan untuk kavling.";
                await this.ShowMessageAsync(
                    "Mohon Periksa Kembali Data", message, MessageDialogStyle.Affirmative, Constants.MDS_OKAY);
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

            if (0 == data.Lots.Count)
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

        /// <summary>
        /// Event handler for a property change event on a <see cref="DataModel.Cost"/> object.
        /// </summary>
        /// <param name="sender">
        /// The sender of the event.
        /// </param>
        /// <param name="e">
        /// The parameters of the event.
        /// </param>
        private void Cost_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            UpdateSecondPageOnDataChange();
        }

        /// <summary>
        /// Event handler for a property change event on an <see cref="System.Collections.ObjectModel"/>
        /// containing <see cref="DataModel.Cost"/> objects.
        /// </summary>
        /// <param name="sender">
        /// The sender of the event.
        /// </param>
        /// <param name="e">
        /// The parameters of the event.
        /// </param>
        private void Costs_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            UpdateSecondPageOnDataChange();
        }

        /// <summary>
        /// Perform updates to the underlying <see cref="DataModel.PropertyDataModel"/> object
        /// when any data on the second page changes.
        /// </summary>
        private void UpdateSecondPageOnDataChange()
        {
            data.Calculate();
            txtTotalCosts.Text = data.TotalCostsOfDevelopment.ToString("#,###.00");
            txtEffectiveLandCost.Text = data.EffectiveLandCost.ToString("#,###.00");
            txtLandResalePrice.Text = data.LandResalePrice.ToString("#,###.00");
        }

        /// <summary>
        /// Perform updates on the mandatory costs whenever necessary.
        /// </summary>
        private void UpdateMandatoryCosts()
        {
            if (null != data.Costs && data.Costs.Count > 0)
            {
                foreach (Cost cost in data.Costs)
                {
                    if (Constants.COST_LAND_PURCHASE.Equals(cost.Name))
                    {
                        cost.Quantity = data.TotalLandArea;
                        cost.UnitValue = data.BaseLandPrice;
                    }

                    if (Constants.COST_PUBLIC_FACILITY.Equals(cost.Name))
                    {
                        cost.Quantity = data.TotalPublicFacilityArea;
                        cost.UnitValue = data.BaseLandPrice;
                    }

                    if (Constants.COST_ROAD_PURCHASE.Equals(cost.Name))
                    {
                        cost.Quantity = data.TotalRoadArea;
                        cost.UnitValue = data.BaseLandPrice;
                    }
                }
            }
        }

        /// <summary>
        /// Assign values to controls on the third and final page on page change to display correct
        /// results of all calculations.
        /// </summary>
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

        /// <summary>
        /// Wrapper method to display a metro style notification. The method has to be async, so using
        /// this method when the answer does not matter can prevent the calling method to have to be
        /// async.
        /// </summary>
        /// <param name="title">
        /// The title of the notification.
        /// </param>
        /// <param name="message">
        /// The content of the notification.
        /// </param>
        /// <param name="style">
        /// The style of the notification, which basically determines what buttons to show.
        /// </param>
        /// <param name="settings">
        /// The settings to be used in display in the notification, which basically determines the wording
        /// on the buttons.
        /// </param>
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

        /// <summary>
        /// Attempt to read an XML from a string that is read from a file. Upon successful reading, the
        /// data is copied to the underlying <see cref="DataModel.PropertyDataModel"/> object and then
        /// loaded to the UI.
        /// </summary>
        /// <param name="attemptedFilename">
        /// The name of the file to read.
        /// </param>
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

        /// <summary>
        /// Copies data from a <see cref="DataModel.PropertyDataModel"/> resulting from a file read to the
        /// underlying <see cref="DataModel.PropertyDataModel"/> used throughout the screen.
        /// </summary>
        /// <remarks>
        /// This is necessary to preserve all data bindings.
        /// </remarks>
        /// <param name="converted">
        /// The <see cref="DataModel.PropertyDataModel"/> that is generated from a file read operation.
        /// </param>
        private void CopyLoadedData(PropertyDataModel converted)
        {
            // Page One
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

            // Page Two
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
        }

        /// <summary>
        /// Transfer the data from the underlying <see cref="DataModel.PropertyDataModel"/> object to appropriate
        /// controls on screen.
        /// </summary>
        /// <remarks>
        /// This is necessary because at the moment the controls on screen are not set to support two-way data binding.
        /// </remarks>
        private void LoadXmlDataToScreen()
        {
            if (null != data)
            {
                // Page One
                if (!string.IsNullOrEmpty(data.Location))
                {
                    txtLocation.Text = data.Location;
                }

                txtLandArea.Text = data.TotalLandArea.ToString("#,##0.00");
                txtRoadArea.Text = data.TotalRoadArea.ToString("#,##0.00");
                txtPublicFacilityArea.Text = data.TotalPublicFacilityArea.ToString("#,##0.00");
                txtBaseLandPrice.Text = data.BaseLandPrice.ToString("#,##0.00");

                // Page Two
                txtTotalCosts.Text = data.TotalCostsOfDevelopment.ToString("#,##0.00");
                txtEffectiveLandCost.Text = data.EffectiveLandCost.ToString("#,##0.00");
                txtLandResaleProfitPercent.Text = data.LandResaleProfitPercent.ToString("#,##0.00");
                txtLandResalePrice.Text = data.LandResalePrice.ToString("#,##0.00");
                txtBuildingPrice.Text = data.BuildingPrice.ToString("#,##0.00");
                txtBuildingPermitCostPerLot.Text = data.BuildingPermitCostPerLot.ToString("#,##0.00");
                txtPromoCostPerLot.Text = data.PromoCostPerLot.ToString("#,##0.00");
                txtValueAddedTaxPercent.Text = data.ValueAddedTaxPercent.ToString("#,##0.00");
                txtFeePercent.Text = data.FeePercent.ToString("#,##0.00");
            }
        }
    }
}