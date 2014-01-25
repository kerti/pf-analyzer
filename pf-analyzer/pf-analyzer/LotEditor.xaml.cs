using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace pf_analyzer
{
    /// <summary>
    /// Interaction logic for LotEditor.xaml
    /// </summary>
    public partial class LotEditor : MetroWindow
    {
        public LotEditor()
        {
            InitializeComponent();
            this.Owner = App.Current.MainWindow;
        }
    }
}
