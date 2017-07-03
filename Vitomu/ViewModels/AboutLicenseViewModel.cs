using Digimezzo.Utilities.IO;
using Digimezzo.Utilities.Log;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;

namespace Vitomu.ViewModels
{
    public class AboutLicenseViewModel : ViewModelBase
    {
        #region Commands
        public RelayCommand<string> OpenLinkCommand { get; set; }
        #endregion

        #region Construction
        public AboutLicenseViewModel()
        {
            this.OpenLinkCommand = new RelayCommand<string>((link) =>
            {
                try
                {
                    Actions.TryOpenLink(link);
                }
                catch (Exception ex)
                {
                    LogClient.Error("Could not open the link {0} in Internet Explorer. Exception: {1}", link, ex.Message);
                }
            });
        }
        #endregion
    }
}
