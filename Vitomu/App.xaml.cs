using Digimezzo.Utilities.IO;
using Digimezzo.Utilities.Log;
using Digimezzo.Utilities.Settings;
using GalaSoft.MvvmLight.Ioc;
using System;
using System.Windows;
using System.Windows.Shell;
using Vitomu.Extensions;
using Vitomu.Services.Convert;
using Vitomu.Services.I18n;
using Vitomu.Services.WindowsIntegration;

namespace Vitomu
{
    public partial class App : Application
    {
        #region Variables
        private II18nService i18nService;
        private IConvertService convertService;
        private IJumpListService jumpListService;
        #endregion

        #region Overrides
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Create a jump-list and assign it to the current application
            JumpList.SetJumpList(Application.Current, new JumpList());

            // Process the command-line arguments
            this.ProcessCommandLineArguments();

            // Make sure services are ready
            this.RegisterServices();
            this.InitializeServices();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            this.convertService.KillProcesses();
        }
        #endregion

        #region Private
        private void ProcessCommandLineArguments()
        {
            // Get the command-line arguments
            string[] args = Environment.GetCommandLineArgs();

            if (args.Length > 1)
            {
                LogClient.Info("Found command-line arguments.");

                if (args[1].Equals("/donate"))
                {
                    LogClient.Info("Detected DonateCommand from JumpList.");

                    try
                    {
                        Actions.TryOpenLink(args[2]);
                    }
                    catch (Exception ex)
                    {
                        LogClient.Error("Could not open the link {0} in Internet Explorer. Exception: {1}", args[2], ex.Message);
                    }
                    this.Shutdown();
                }
            }
        }

        private void RegisterServices()
        {
            SimpleIoc.Default.RegisterOnce<II18nService, I18nService>();
            SimpleIoc.Default.RegisterOnce<IConvertService, ConvertService>();
            SimpleIoc.Default.RegisterOnce<IJumpListService, JumpListService>();
        }

        private void InitializeServices()
        {
            this.i18nService = SimpleIoc.Default.GetInstance<II18nService>();
            this.i18nService.ApplyLanguageAsync(SettingsClient.Get<string>("Configuration", "Language")); // Set default language
            this.convertService = SimpleIoc.Default.GetInstance<IConvertService>();
            this.jumpListService = SimpleIoc.Default.GetInstance<IJumpListService>();
            this.jumpListService.PopulateJumpListAsync();
        }
        #endregion
    }
}
