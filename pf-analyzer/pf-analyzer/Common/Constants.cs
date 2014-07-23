using MahApps.Metro.Controls.Dialogs;

namespace PFAnalyzer.Common
{
    /// <summary>
    /// A class containing all constants that are to be used throughout the project.
    /// </summary>
    public static class Constants
    {
        #region File Type

        /// <summary>
        /// The file extension that is to be associated with PF Analyzer files.
        /// </summary>
        public static readonly string FILE_EXTENSION = ".pfz";

        /// <summary>
        /// The description of PF Analyzer files that is to be displayed in Open File Dialogs.
        /// </summary>
        public static readonly string FILE_TYPE = "PFZ Files";

        #endregion

        #region Cost Name Constants

        /// <summary>
        /// The display name of the Mandatory Cost of Land Purchase.
        /// </summary>
        public static readonly string COST_LAND_PURCHASE = "Harga Beli Tanah";

        /// <summary>
        /// The display name of the Mandatory Cost of Road Purchase.
        /// </summary>
        public static readonly string COST_ROAD_PURCHASE = "Biaya Tanah untuk Jalan";

        /// <summary>
        /// The display name of the Mandatory Cost of Land Allocated for Public Facilities.
        /// </summary>
        public static readonly string COST_PUBLIC_FACILITY = "Biaya Tanah untuk Fasilitas Umum";

        #endregion

        #region Dialog Screen Settings

        /// <summary>
        /// The default settings for a single-button metro dialog.
        /// </summary>
        public static readonly MetroDialogSettings MDS_OKAY = new MetroDialogSettings()
        {
            AffirmativeButtonText = "Baiklah"
        };

        /// <summary>
        /// The default settings for a two-button yes-no metro dialog.
        /// </summary>
        public static readonly MetroDialogSettings MDS_YESNO = new MetroDialogSettings()
        {
            AffirmativeButtonText = "Ya",
            NegativeButtonText = "Tidak"
        };

        /// <summary>
        /// The default settings for a three-button yes-no-cancel metro dialog.
        /// </summary>
        public static readonly MetroDialogSettings MDS_YESNOCANCEL = new MetroDialogSettings()
        {
            AffirmativeButtonText = "Ya",
            NegativeButtonText = "Tidak",
            FirstAuxiliaryButtonText = "Batal"
        };

        #endregion
    }
}
