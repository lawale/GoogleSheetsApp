using GoogleSheetsApp.Services;
using GoogleSheetsApp.Views;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using XF.Material.Forms.UI;

namespace GoogleSheetsApp
{
    public partial class App : Application
    {
        public App()
        {
            //Device.SetFlags(new string[] { "RadioButton_Experimental" });
            InitializeComponent();
            XF.Material.Forms.Material.Init(this);
            MainPage = new MaterialNavigationPage(new FormsPage());
        }



        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
