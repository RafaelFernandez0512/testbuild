using System;
using System.Collections.Generic;
using SigeMobile.Model;
using SigeTools.ViewModel;
using Xamarin.Forms;

namespace SigeTools.View
{
    public partial class ConsultaInventarioPage : ContentPage
    {
        public ConsultaInventarioPage(Producto producto = null)
        {
            InitializeComponent();
            BindingContext = new ConsultaInventarioPageViewModel(producto);
        }
    }
}
