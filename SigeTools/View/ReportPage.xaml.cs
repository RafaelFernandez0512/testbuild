using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SigeMobile.Model;
using SigeTools.ViewModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SigeTools.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ReportPage : ContentPage
    {
        public ReportPage(MenuReport menuReport)
        {
            var vw = new ReportPageViewModel();
            vw.Init(menuReport.Id);
            Title = menuReport.Title;
            BindingContext = vw;
            InitializeComponent();
            
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (BindingContext is ReportPageViewModel vw)
            {
                vw.LoadDataCommand.Execute(null);
            }
        }
    }
}