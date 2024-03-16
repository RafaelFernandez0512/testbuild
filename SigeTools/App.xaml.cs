using System;
using GoogleVisionBarCodeScanner;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using SigeTools.View;

namespace SigeTools
{
    public partial class App : Application
    {
        public App()
        {
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("NDI5NjYxQDMxMzkyZTMxMmUzMEtDRXlIYndldkJvMHZQU2RqVjEvVnU2dTNIWkpEdmpXdUx2Uld0anlwT0k9");
            InitializeComponent();
            GoogleVisionBarCodeScanner.Methods.SetSupportBarcodeFormat(BarcodeFormats.All);
            MainPage = new LoginPage();// MainPage();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
