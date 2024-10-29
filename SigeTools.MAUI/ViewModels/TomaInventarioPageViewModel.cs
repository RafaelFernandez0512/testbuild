
using System.Windows.Input;

using Controls.UserDialogs.Maui;
using SigeTools.MAUI;
using SigeTools.MAUI.Helpers;
using SigeTools.MAUI.Model;
using SigeTools.MAUI.Services;
using SigeTools.MAUI.ViewModels;

namespace SigeTools.ViewModel
{
    public class TomaInventarioPageViewModel : BaseViewModel
    {
        public ICommand StopScanningCommand
        {
            get
            {
                return new Command(SetConsultaMode);
            }
        }

        public ICommand RemoveLineCommand
        {
            get
            {
                return new Command((obj) =>
                {
                    var line = (obj as OrdenDetalle);
                    if (line != null)
                    {
                        Orden.Lineas.Remove(line);
                        Orden.SetDirty();
                    }
                });
            }
        }
        
        void SetBusy(bool value)
        {
            IsBusy = value;
            IsNotBusy = !value;
        }

        public Boolean ShowScanner { get; set; }

        public Boolean UseTorch { get; set; }
        public Boolean UseVibration { get; set; }
        public string Upc { get; set; }

        public Orden Orden { get; set; }

        readonly ISigeService sigeService;

        public ICommand ScanDetectedCommand { get; set; }
        public ICommand ContinueScanningCommand { get; set; }
        public ICommand ToogleScannerCommand { get; set; }
        public ICommand SearchCommand { get; set; }
        public IAsyncCommand SendLineCommand { get; set; }

        public String Almacen { get; set; }

        public bool IsScanning { get; set; }

        private string _locationId;
        private readonly IUserDialogs _dlg;

        public TomaInventarioPageViewModel(INavigationService navigationService,ISigeService sigeService,IUserDialogs userDialog,IPageDialogService pageDialogService) : base(navigationService,pageDialogService)
        {
            Orden = new Orden();
            //
            Almacen = AppSettings.CurrentStore;
            UseTorch = false;
            UseVibration = true;
            this.sigeService = sigeService;
            ScanDetectedCommand = new Command(DoScanDetected);
            _dlg = userDialog;
            IsScanning = true;
            ContinueScanningCommand = new Command(SetScanMode);
            ToogleScannerCommand = new Command(() =>
            {
                ShowScanner = !ShowScanner;
            });
            SearchCommand = new Command(DoSearch);
            SendLineCommand = new AsyncDelegateCommand(SendLine);
        }

        void SetScanMode()
        {
            IsScanning = true;
            //GoogleVisionBarCodeScanner.Methods.SetIsScanning(true);

        }

        void SetConsultaMode()
        {
            IsScanning = false;
           // GoogleVisionBarCodeScanner.Methods.SetIsScanning(false);
        }

        public string UpcText { get; set; }

        public string LocationId
        {
            get => _locationId;
            set
            {
                if (value == _locationId) return;
                _locationId = value;
            }
        }

        private async void DoSearch()
        {
            if (IsBusy) return;

            if (String.IsNullOrEmpty(UpcText))
            {
                return;
            }


            try
            {
                SetBusy(true);

                var upc = UpcText;
                var producto = await sigeService.GetProducto(upc);

                SetBusy(false);

                UpcText = String.Empty;
                if (producto != null)
                {
                    AddOrUpdateProductoInOden(producto);
                }
                else
                {
                    _dlg.Alert($"No se encontro producto para el código: '{upc}'", "Aviso", "Aceptar");
                }

            }
            catch (Exception ex)
            {
                _dlg.ShowToast("Eror en proceso: " + ex.Message);
            }
            finally
            {
                SetBusy(false);
            }
            
        }

        private async void DoScanDetected(object sender)
        {
            // //GoogleVisionBarCodeScanner.OnDetectedEventArg e = (obj as GoogleVisionBarCodeScanner.OnDetectedEventArg);
            // if (e != null)
            // {
            //     List<GoogleVisionBarCodeScanner.BarcodeResult> scanResults = e.BarcodeResults;
            //     if (scanResults.Any())
            //     {
            //         try
            //         {
            //             var upc = scanResults.First().DisplayValue;
            //
            //             var producto = await sigeService.GetProducto(upc);
            //
            //             if (producto != null)
            //             {
            //                 AddOrUpdateProductoInOden(producto);
            //             }
            //             else
            //             {
            //                 _dlg.Alert($"No se encontro producto para el código: '{upc}'", "Aviso", "Aceptar");
            //             }
            //         }
            //         catch (Exception ex)
            //         {
            //             _dlg.Alert($"Ha ocurrido un error en el proceso:{ex.Message}", "Alerta", "Aceptar");
            //         }
            //     }
            // }
          //  GoogleVisionBarCodeScanner.Methods.SetIsScanning(true);
        }

        async Task SendLine()
        {
            try
            {
                if (!await _dlg.ConfirmAsync(new ConfirmConfig
                    {
                        Title = "Confirme", Message = "Seguro que desea enviar el conteo de inventario?",
                        OkText = "Enviar", CancelText = "Cancelar"
                    }))
                {
                    return;
                }

                _dlg.ShowLoading();
                var lines = Orden.Lineas.ToList();
                var linesSend = new List<OrdenDetalle>();
                foreach (var line in lines)
                {

                    var hasSend = await sigeService.CreateInventoryCount(LocationId, line.Upc, line.Cantidad);
                    if (hasSend)
                    {
                        linesSend.Add(line);

                    }
                }

                foreach (var line in linesSend)
                {
                    Orden.Lineas.Remove(line);
                }

                _dlg.HideHud();
            }
            catch (Exception e)
            {
                _dlg.HideHud();
                await App.Current.MainPage.DisplayAlert("Error", e.Message, "ok");

            }
        }

        private void AddOrUpdateProductoInOden(Producto producto)
        {
            var linea = Orden.Lineas.FirstOrDefault(x => x.ProdutoId == producto.ProductoId);
            if (linea == null)
            {
                linea = new OrdenDetalle
                {
                    ProdutoId = producto.ProductoId,
                    Upc = producto.Upc,
                    Descripcion = producto.Nombre,
                    Cantidad = 1,
                    Precio = producto.Precio,
                    
                };
                Orden.Lineas.Add(linea);
            }
            else
            {
                linea.Cantidad = linea.Cantidad + 1;
            }
            linea.SetDirty();
            Orden.SetDirty();
        }
    }
}
