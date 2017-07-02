using System.Threading.Tasks;
using Vitomu.Controls;
using Vitomu.Views;

namespace Vitomu
{
    public partial class Shell : VitomuWindow
    {
        public Shell()  
        {
            InitializeComponent();
            this.ShowWindowControls = false;
            this.ShellFrame.Navigate(new Splash());
        }

        #region Event handlers
        private async void Shell_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            await Task.Delay(1000); // Small delay for the splash screen
            this.ShellFrame.Navigate(new Main());
            this.ShowWindowControls = true;
        }
        #endregion  
    }
}
