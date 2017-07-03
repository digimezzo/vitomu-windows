using Digimezzo.Utilities.IO;
using Digimezzo.Utilities.Packaging;
using Digimezzo.Utilities.Utils;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using Vitomu.Services.Dialog;
using Vitomu.Views;

namespace Vitomu.ViewModels
{
    public class AboutViewModel : ViewModelBase
    {
        #region Variables
        private Package package;
        private IDialogService dialogService;
        #endregion

        #region Commands
        public RelayCommand ShowLicenseCommand { get; set; }
        #endregion

        #region Properties
        public Package Package
        {
            get { return this.package; }
            set {
                this.package = value;
                RaisePropertyChanged(() => this.Package);
            }
        }
        #endregion

        #region Construction
        public AboutViewModel(IDialogService dialogService)
        {
            this.dialogService = dialogService;

            Configuration config;
#if DEBUG
            config = Configuration.Debug;
#else
		    config = Configuration.Release;
#endif

            this.Package = new Package(ProcessExecutable.Name(), ProcessExecutable.AssemblyVersion(), config);

            this.ShowLicenseCommand = new RelayCommand(() =>
            {
                AboutLicense view = SimpleIoc.Default.GetInstance<AboutLicense>();

                this.dialogService.ShowCustomDialog(
                    ResourceUtils.GetStringResource("Language_License"),
                    view,
                    400,
                    0,
                    false,
                    true,
                    false,
                    ResourceUtils.GetStringResource("Language_Ok"),
                    string.Empty,
                    null);
            });
        }
        #endregion
    }
}
