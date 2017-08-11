using Digimezzo.Utilities.Helpers;
using Digimezzo.Utilities.Settings;
using GalaSoft.MvvmLight;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Vitomu.Base;

namespace Vitomu.ViewModels
{
    public class SettingsViewModel : ViewModelBase
    {
        #region Variables
        private ObservableCollection<AudioFormat> audioFormats;
        private AudioFormat selectedAudioFormat;
        private ObservableCollection<NameValue> bitRates;
        private NameValue selectedBitRate;
        #endregion

        #region Properties
        public ObservableCollection<AudioFormat> AudioFormats
        {
            get { return this.audioFormats; }
            set
            {
                this.audioFormats = value;
                RaisePropertyChanged(() => this.AudioFormats);
            }
        }

        public AudioFormat SelectedAudioFormat
        {
            get { return this.selectedAudioFormat; }
            set
            {
                this.selectedAudioFormat = value;
                RaisePropertyChanged(() => this.SelectedAudioFormat);
                SettingsClient.Set<string>("Output", "Format", value.Id);
            }
        }

        public ObservableCollection<NameValue> BitRates
        {
            get { return this.bitRates; }
            set
            {
                this.bitRates = value;
                RaisePropertyChanged(() => this.BitRates);
            }
        }

        public NameValue SelectedBitRate
        {
            get { return this.selectedBitRate; }
            set
            {
                this.selectedBitRate = value;
                RaisePropertyChanged(() => this.SelectedBitRate);
                SettingsClient.Set<int>("Output", "BitRate", value.Value);
            }
        }
        #endregion

        #region Construction
        public SettingsViewModel()
        {
            this.GetAudioFormatsAsync();
            this.GetBitRatesAsync();
        }
        #endregion

        #region Private
        private async void GetAudioFormatsAsync()
        {
            var localAudioFormats = new ObservableCollection<AudioFormat>();

            await Task.Run(() =>
            {
                foreach (AudioFormat format in FileFormats.SupportedAudioFormats.OrderBy(f => f.Name))
                {
                    localAudioFormats.Add(format);
                }
            });

            this.AudioFormats = localAudioFormats;

            AudioFormat localSelectedAudioFormat = null;

            await Task.Run(() => localSelectedAudioFormat = FileFormats.GetValidAudioFormat(SettingsClient.Get<string>("Output", "Format")));
            this.SelectedAudioFormat = localSelectedAudioFormat;
        }

        private async void GetBitRatesAsync()
        {
            var localBitRates = new ObservableCollection<NameValue>();
            string bitRateUnit = "kbps";

            await Task.Run(() =>
            {
                localBitRates.Add(new NameValue() { Name = $"32 {bitRateUnit}", Value = 32 });
                localBitRates.Add(new NameValue() { Name = $"48 {bitRateUnit}", Value = 48 });
                localBitRates.Add(new NameValue() { Name = $"56 {bitRateUnit}", Value = 56 });
                localBitRates.Add(new NameValue() { Name = $"64 {bitRateUnit}", Value = 64 });
                localBitRates.Add(new NameValue() { Name = $"96 {bitRateUnit}", Value = 96 });
                localBitRates.Add(new NameValue() { Name = $"128 {bitRateUnit}", Value = 128 });
                localBitRates.Add(new NameValue() { Name = $"160 {bitRateUnit}", Value = 160 });
                localBitRates.Add(new NameValue() { Name = $"192 {bitRateUnit}", Value = 192 });
                localBitRates.Add(new NameValue() { Name = $"224 {bitRateUnit}", Value = 224 });
                localBitRates.Add(new NameValue() { Name = $"256 {bitRateUnit}", Value = 256 });
                localBitRates.Add(new NameValue() { Name = $"320 {bitRateUnit}", Value = 320 });
            });

            this.BitRates = localBitRates;

            NameValue localSelectedBitRate = null;
            int bitRate = FileFormats.GetValidBitRate(SettingsClient.Get<int>("Output", "BitRate"));
            await Task.Run(() => localSelectedBitRate = new NameValue() { Name = $"{bitRate} {bitRateUnit}", Value = bitRate });
            this.SelectedBitRate = localSelectedBitRate;
        }
        #endregion
    }
}
