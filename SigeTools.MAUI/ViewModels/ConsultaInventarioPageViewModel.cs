using System.Windows.Input;
using Controls.UserDialogs.Maui;
using SigeTools.MAUI.Model;
using SigeTools.MAUI.Services;

namespace SigeTools.MAUI.ViewModels
{
    public class ConsultaInventarioPageViewModel : BaseViewModel
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
        
        void SetBusy(bool value)
        {
            IsBusy = value;
            IsNotBusy = !value;
        }

        public ConsultaInventarioPageViewModel(INavigationService navigationService, ISigeService sigeService,
            IPageDialogService pageDialogService,IUserDialogs userDialogs) : base(navigationService,pageDialogService)
        {
            UseTorch = false;
            UseVibration = true;
            this.sigeService = sigeService;
            ToogleScannerCommand = new Command(() =>
            {
                ShowScanner = !ShowScanner;
              //  GoogleVisionBarCodeScanner.Methods.SetIsScanning(ShowScanner);
            });
            SearchCommand = new Command(DoSearch);
            ScanDetectedCommand = new Command(DoScanDetected);
            IsScanning = true;
            ContinueScanningCommand = new Command(SetScanMode);
            this.userDialogs = userDialogs;


        }

        public override async Task InitializeAsync(INavigationParameters parameters)
        {
            await  base.InitializeAsync(parameters);
            var producto = parameters.GetValue<Producto?>("Producto");
            if (producto!=null)
            {
                UpcText = producto.Upc;
                SearchCommand.Execute(null);

            }
        }

        void SetScanMode()
        {
            Resultado = null;
            IsScanning = true;
            // OnPropertyChanged(nameof(IsScanning));
            // OnPropertyChanged(nameof(Resultado));
        }

        void SetConsultaMode(ConsultaInventario consultaInventario)
        {
            Resultado = consultaInventario;
            IsScanning = false;
            // OnPropertyChanged(nameof(IsScanning));
            // OnPropertyChanged(nameof(Resultado));
        }
        private async void DoSearch()
        {
            if (IsBusy) return;

            if(String.IsNullOrEmpty(UpcText))
            {
                userDialogs.ShowToast("Nada que buscar");
                return;
            }

            try
            {
                SetBusy(true);

                var upc = UpcText;
                var result = await sigeService.GetConsultaInventario(upc);

                SetBusy(false);
                UpcText = string.Empty;

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
                userDialogs.ShowToast("Error en proceso: " + ex.Message);
            } finally
            {
                SetBusy(false);
            }

            

        }
        private async void DoScanDetected(object obj)
        {
            // GoogleVisionBarCodeScanner.OnDetectedEventArg e = (obj as GoogleVisionBarCodeScanner.OnDetectedEventArg);
            // if (e != null)
            // {
            //     List<GoogleVisionBarCodeScanner.BarcodeResult> scanResults = e.BarcodeResults;
            //     if (scanResults.Any())
            //     {
            //         UpcText = scanResults.First().DisplayValue;
            //         //
            //         ShowScanner = false;
            //         GoogleVisionBarCodeScanner.Methods.SetIsScanning(false);
            //         OnPropertyChanged(nameof(ShowScanner));
            //         DoSearch();
            //         return;
            //     }
            // };
            // GoogleVisionBarCodeScanner.Methods.SetIsScanning(true);
            // return;
        }
    }
}
