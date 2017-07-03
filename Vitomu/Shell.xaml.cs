using Digimezzo.Utilities.IO;
using Digimezzo.Utilities.Log;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
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

        private void VitomuWindow_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Control & e.Key == Key.L)
            {
                try
                {
                    Actions.TryViewInExplorer(LogClient.Logfile()); // View the log file
                }
                catch (Exception ex)
                {
                    LogClient.Error("Could not view the log file {0} in explorer. Exception: {1}", LogClient.Logfile(), ex.Message);
                }
            }
        }
    }
}
