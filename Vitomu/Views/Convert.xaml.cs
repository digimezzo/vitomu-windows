using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Vitomu.Views
{
    public partial class Convert : UserControl
    {
        #region Construction
        public Convert()
        {
            InitializeComponent();
        }
        #endregion

        private void ConvertControl_Loaded(object sender, RoutedEventArgs e)
        {
            this.Focus();
        }
    }
}
