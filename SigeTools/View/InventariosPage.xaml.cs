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
    public partial class InventariosPage : ContentPage
    {
        public InventariosPage()
        {
            InitializeComponent();
            BindingContext = new InventariosViewModel();
        }
    }
}