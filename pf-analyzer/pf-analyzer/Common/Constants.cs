using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pf_analyzer.Common
{
    public static class Constants
    {

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
    }
}
