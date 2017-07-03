using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;

namespace Vitomu.ViewModels
{
    public class ViewModelLocator
    {
        #region Construction
        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);
            this.RegisterViewModels();
        }
        #endregion

        #region Properties
        public MainViewModel Main => ServiceLocator.Current.GetInstance<MainViewModel>();
        public AboutViewModel About => ServiceLocator.Current.GetInstance<AboutViewModel>();
        public AboutLicenseViewModel AboutLicense => ServiceLocator.Current.GetInstance<AboutLicenseViewModel>();
        public ConvertViewModel Convert => ServiceLocator.Current.GetInstance<ConvertViewModel>();
        #endregion

        #region Static
        public static void Cleanup()
        {
            // TODO Clear the ViewModels
        }
        #endregion

        #region Private
        private void RegisterViewModels()
        {
            SimpleIoc.Default.Register<MainViewModel>();
            SimpleIoc.Default.Register<AboutViewModel>();
            SimpleIoc.Default.Register<AboutLicenseViewModel>();
            SimpleIoc.Default.Register<ConvertViewModel>();
        }
        #endregion
    }
}