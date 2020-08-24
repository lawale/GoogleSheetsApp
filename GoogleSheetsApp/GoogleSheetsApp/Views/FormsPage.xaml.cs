using GoogleSheetsApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GoogleSheetsApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FormsPage : ContentPage
    {
        public FormsPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            var vm = BindingContext as FormsViewModel;
            vm.LoadCommand.Execute(null);
        }

    }
}