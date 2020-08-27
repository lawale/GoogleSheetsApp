using GoogleSheetsApp.Services;
using GoogleSheetsApp.Views;
using Matcha.BackgroundService;
using Xamarin.Forms;
using XF.Material.Forms;
using XF.Material.Forms.UI;
using XF.Material.Forms.UI.Dialogs;
using XF.Material.Forms.UI.Dialogs.Configurations;

namespace GoogleSheetsApp
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            Material.Init(this);
            InitializeMaterialConfig();
            MainPage = new MaterialNavigationPage(new FormsPage());
        }

        private void InitializeMaterialConfig()
        {
            var alertDialogConfiguration = new MaterialAlertDialogConfiguration
            {
                BackgroundColor = Color.FromHex("#29357A"),
                TitleTextColor = Color.White,
                MessageTextColor = Color.White.MultiplyAlpha(0.8),
                TintColor = Color.White,
                CornerRadius = 8,
                ScrimColor = Color.FromHex("#232F34").MultiplyAlpha(0.32),
                ButtonAllCaps = true
            };

            var loadingDialogConfiguration = new MaterialLoadingDialogConfiguration
            {
                BackgroundColor = Color.FromHex("#29357A"),
                MessageTextColor = Color.White.MultiplyAlpha(0.8),
                TintColor = Color.White,
                CornerRadius = 8,
                ScrimColor = Color.FromHex("#232F34").MultiplyAlpha(0.32)
            };
            MaterialDialog.Instance.SetGlobalStyles(alertDialogConfiguration, loadingDialogConfiguration);
        }


        protected override void OnStart()
        {
            // Handle when your app starts
            BackgroundAggregatorService.Add(() => new BackgroundRetryService());
        }

        protected override void OnSleep()
        {
            BackgroundAggregatorService.StartBackgroundService();
        }

        protected override void OnResume()
        {
            BackgroundAggregatorService.StopBackgroundService();
        }
    }
}
