using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace SigeTools.View
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        //private void CameraView_OnDetected(object sender, GoogleVisionBarCodeScanner.OnDetectedEventArg e)
        //{
        //    List<GoogleVisionBarCodeScanner.BarcodeResult> obj = e.BarcodeResults;

        //    string result = string.Empty;
        //    for (int i = 0; i < obj.Count; i++)
        //    {
        //        result += $"{i + 1}. Type : {obj[i].BarcodeType}, Value : {obj[i].DisplayValue}{Environment.NewLine}";
        //    }
        //    Device.BeginInvokeOnMainThread(async () =>
        //    {
        //        await DisplayAlert("Result", result, "OK");
        //        //If you want to stop scanning, you can close the scanning page
        //        // await Navigation.PopModalAsync();
        //        //if you want to keep scanning the next barcode, do not close the scanning page and call below function
        //        GoogleVisionBarCodeScanner.Methods.SetIsScanning(true);
        //    });

        //}
    }
}
