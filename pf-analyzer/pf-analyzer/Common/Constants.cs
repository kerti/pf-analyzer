using MahApps.Metro.Controls.Dialogs;

namespace pf_analyzer.Common
{
    public static class Constants
    {

        #region File Type

        public static readonly string FILE_EXTENSION = ".pfz";
        public static readonly string FILE_TYPE = "PFZ Files";

        #endregion

        #region Cost Name Constants

        public static readonly string COST_LAND_PURCHASE = "Harga Beli Tanah";
        public static readonly string COST_ROAD_PURCHASE = "Biaya Tanah untuk Jalan";
        public static readonly string COST_PUBLIC_FACILITY = "Biaya Tanah untuk Fasilitas Umum";

        #endregion

        #region Dialog Screen Settings

        public static readonly MetroDialogSettings MDS_OKAY = new MetroDialogSettings()
        {
            AffirmativeButtonText = "Baiklah"
        };

        public static readonly MetroDialogSettings MDS_YESNO = new MetroDialogSettings()
        {
            AffirmativeButtonText = "Ya",
            NegativeButtonText = "Tidak"
        };

        public static readonly MetroDialogSettings MDS_YESNOCANCEL = new MetroDialogSettings()
        {
            AffirmativeButtonText = "Ya",
            NegativeButtonText = "Tidak",
            FirstAuxiliaryButtonText = "Batal"
        };

        #endregion

    }
}
