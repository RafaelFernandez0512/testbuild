using System;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;
using System.Windows.Input;
using System.Collections.Generic;
using Acr.UserDialogs;
using SigeTools.Services;
using SigeMobile.Model;
using System.Linq;

namespace SigeTools.ViewModel
{
    public class ConsultaInventarioPageViewModel : ObservableObject
    {
        public Boolean UseTorch { get; set; }
        public Boolean UseVibration { get; set; }
        public string Upc { get; set; }

        public ConsultaInventario Resultado { get; set; }

        readonly ISigeService sigeService;

        public ICommand ScanDetectedCommand { get; set; }
        public ICommand ContinueScanningCommand { get; set; }

        public bool IsScanning { get; set; }

        IUserDialogs userDialogs;

        public bool ShowScanner { get; set; }

        public ICommand ToogleScannerCommand { get; set; }

        public string UpcText { get; set; }

        public ICommand SearchCommand { get; set; }

        private bool _isBusy = false;
        public Boolean IsBusy
        {
            get => _isBusy;
            set => SetProperty(ref _isBusy, value);
        }

        private bool _isNotBusy = true;
        public Boolean IsNotBusy
        {
            get => _isNotBusy;
            set => SetProperty(ref _isNotBusy, value);
        }
        void SetBusy(bool value)
        {
            IsBusy = value;
            IsNotBusy = !value;
        }

        public ConsultaInventarioPageViewModel(Producto producto = null)
        {
            UseTorch = false;
            UseVibration = true;
            sigeService = ServiceFactory.SigeService;
            ToogleScannerCommand = new Command(() =>
            {
                ShowScanner = !ShowScanner;
                OnPropertyChanged(nameof(ShowScanner));
                GoogleVisionBarCodeScanner.Methods.SetIsScanning(ShowScanner);
            });
            SearchCommand = new Command(DoSearch);
            ScanDetectedCommand = new Command(DoScanDetected);
            IsScanning = true;
            ContinueScanningCommand = new Command(SetScanMode);
            userDialogs = Acr.UserDialogs.UserDialogs.Instance;
            if (producto!=null)
            {
                UpcText = producto?.Upc;
                SearchCommand.Execute(null);

            }

        }

        void SetScanMode()
        {
            Resultado = null;
            IsScanning = true;
            OnPropertyChanged(nameof(IsScanning));
            OnPropertyChanged(nameof(Resultado));
        }

        void SetConsultaMode(ConsultaInventario consultaInventario)
        {
            Resultado = consultaInventario;
            IsScanning = false;
            OnPropertyChanged(nameof(IsScanning));
            OnPropertyChanged(nameof(Resultado));
        }
        private async void DoSearch()
        {
            if (IsBusy) return;

            if(String.IsNullOrEmpty(UpcText))
            {
                userDialogs.Toast("Nada que buscar");
                return;
            }

            try
            {
                SetBusy(true);

                var upc = UpcText;
                var result = await sigeService.GetConsultaInventario(upc);

                SetBusy(false);

                UpcText = String.Empty;
                OnPropertyChanged(nameof(UpcText));

                if (result != null)
                {
                    SetConsultaMode(result);
                    return;
                }
                else
                {
                    userDialogs.Alert($"No se encontro producto para el código: '{upc}'", "Aviso", "Aceptar");
                    SetScanMode();
                }

            }
            catch (Exception ex)
            {
                userDialogs.Toast("Error en proceso: " + ex.Message);
            } finally
            {
                SetBusy(false);
            }

            

        }
        private async void DoScanDetected(object obj)
        {
            GoogleVisionBarCodeScanner.OnDetectedEventArg e = (obj as GoogleVisionBarCodeScanner.OnDetectedEventArg);
            if (e != null)
            {
                List<GoogleVisionBarCodeScanner.BarcodeResult> scanResults = e.BarcodeResults;
                if (scanResults.Any())
                {
                    UpcText = scanResults.First().DisplayValue;
                    //
                    ShowScanner = false;
                    GoogleVisionBarCodeScanner.Methods.SetIsScanning(false);
                    OnPropertyChanged(nameof(ShowScanner));
                    DoSearch();
                    return;
                }
            };
            GoogleVisionBarCodeScanner.Methods.SetIsScanning(true);
            return;
        }
    }
}
