using GalaSoft.MvvmLight;
using Vitomu.Base;

namespace Vitomu.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        #region Properties
        public string ApplicationDisplayName => ProductInformation.ApplicationDisplayName;
        #endregion
    }
}
