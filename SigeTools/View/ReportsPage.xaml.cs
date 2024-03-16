using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SigeTools.ViewModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SigeTools.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ReportsPage : ContentPage
    {
        public ReportsPage()
        {
            BindingContext = new ReportsViewModel();
            InitializeComponent();
        }
    }
}