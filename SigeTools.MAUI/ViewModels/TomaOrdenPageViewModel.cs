
using System.Windows.Input;

using Controls.UserDialogs.Maui;
using SigeTools.MAUI;
using SigeTools.MAUI.Helpers;
using SigeTools.MAUI.Model;
using SigeTools.MAUI.Services;
using SigeTools.MAUI.ViewModels;


namespace SigeTools.ViewModel
{
    public class TomaOrdenPageViewModel : BaseViewModel
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
                   if(line != null)
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

        readonly ISigeService _sigeService;

        public ICommand ScanDetectedCommand { get; set; }
        public ICommand ContinueScanningCommand { get; set; }
        public ICommand ToogleScannerCommand { get; set; }
        public ICommand SearchCommand { get; set; }
        public ICommand SaveOrderCommand { get; set; }

        public bool IsScanning { get; set; }

        IUserDialogs userDialogs;
        private readonly DeviceStorageService _deviceStorageService;
        public ICommand SharePdfQuote { get; set; }

        public TomaOrdenPageViewModel(INavigationService navigationService,IUserDialogs userDialog,ISigeService sigeService,IPageDialogService pageDialogService) :
            base(navigationService,pageDialogService)
        {
            Orden = new Orden();
            //
            UseTorch = false;
            UseVibration = true;
            _sigeService =sigeService;
            ScanDetectedCommand = new Command(DoScanDetected);
            IsScanning = true;
            ContinueScanningCommand = new Command(SetScanMode);
            ToogleScannerCommand = new Command(() =>
          {
              ShowScanner = !ShowScanner;
              //GoogleVisionBarCodeScanner.Methods.SetIsScanning(ShowScanner);
          });
            SaveOrderCommand = new Command(DoSave);
            SearchCommand = new Command(DoSearch);
            SharePdfQuote = new Command(async ()=>await PdfQuote());
            userDialogs = userDialog;
            _deviceStorageService = new DeviceStorageService();
        }

        public async Task ShareFile(string title, MemoryStream memoryStream,string fileName)
        {
            try
            {
                var file =  await _deviceStorageService.Write(memoryStream, fileName);
                await Share.RequestAsync(new ShareFileRequest()
                {
                    Title = title,
                    File = new ShareFile(file),
                });
            }
            catch (Exception e)  
            {
                Console.WriteLine(e);
            }
        }

      async  Task PdfQuote()
        {
            try
            {
                if (IsBusy) return;
                if (!Orden.Lineas.Any())
                {
                    userDialogs.Alert($"Nada que enviar.", "Alerta", "Aceptar");
                    return;
                }

                var dlgResult = await PageDialogService.DisplayPromptAsync("Nombre del cliente", "Registre", "Aceptar", "Cancelar");
                

                if (string.IsNullOrEmpty(dlgResult))
                {
                    userDialogs.Alert($"Debe especificar un nombre de cliente.", "Alerta", "Aceptar");
                    return;
                }

                userDialogs.ShowLoading();
                SetBusy(true);
                Orden.CustomerName = "Cliente";
                Orden.Transtype = "QTO";
                var orderId = await _sigeService.SaveOrden(Orden, $"{ dlgResult}");
                Orden.OrdenId = orderId;
                var quote = _sigeService.QuotePdf(Orden);
                var name = $"{DateTime.Today:ddMMyy}_{Orden.CustomerName}_Cuota";
                userDialogs.HideHud();
                await ShareFile(name, new MemoryStream(quote), name);

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                userDialogs.HideHud();;
            }
            finally
            {
                SetBusy(false);
                userDialogs.HideHud();;
            }
        }
        private async void DoSave(object obj)
        {
            if (IsBusy) return;
            if(!Orden.Lineas.Any())
            {
                userDialogs.Alert($"Nada que enviar.", "Alerta", "Aceptar");
                return;
            }
            if(!await userDialogs.ConfirmAsync(new ConfirmConfig { Title = "Confirme", Message = "Seguro que desea enviar el pedido?", OkText = "Enviar", CancelText = "Cancelar" }))
            {
                return;
            }
            var dlgResult = await PageDialogService.DisplayPromptAsync("Nombre del cliente", "Registre", "Aceptar", "Cancelar");

            if(string.IsNullOrEmpty(dlgResult))
            {
                userDialogs.Alert($"Debe especificar un nomnbre de cliente.", "Alerta", "Aceptar");
                return;
            }

            try
            {
                SetBusy(true);
                var orderId = await _sigeService.SaveOrden(Orden, dlgResult);
                if (!String.IsNullOrEmpty(orderId))
                {
                    await userDialogs.AlertAsync($"Se ha creado el pedido '{orderId}'.", "Aviso", "Aceptar");
                }

                await NavigationService.GoBackAsync();

            }
            catch (Exception ex)
            {
                userDialogs.Alert($"Ha ocurrido un error en el proceso: '{ex.Message}'", "Alerta", "Aceptar");
            }
            finally
            {
                SetBusy(false);
            }
        }

        void SetScanMode()
        {
            IsScanning = true;
            // GoogleVisionBarCodeScanner.Methods.SetIsScanning(true);
            // OnPropertyChanged(nameof(IsScanning));
            // OnPropertyChanged(nameof(Orden));
        }

        void SetConsultaMode()
        {
            IsScanning = false;
            // GoogleVisionBarCodeScanner.Methods.SetIsScanning(false);
            // OnPropertyChanged(nameof(IsScanning));
            // OnPropertyChanged(nameof(Orden));
        }

        public string UpcText { get; set; }
        private async void DoSearch()
        {
            if (IsBusy) return;


            try
            {
                SetBusy(true);

                var upc = UpcText;
                var producto = await _sigeService.GetProducto(upc);

                SetBusy(false);

                UpcText = string.Empty;

                if (producto != null)
                {
                    AddOrUpdateProductoInOden(producto);
                }
                else
                {
                    userDialogs.Alert($"No se encontro producto para el código: '{upc}'", "Aviso", "Aceptar");
                }

            }
            catch (Exception ex)
            {
                userDialogs.ShowToast("Eror en proceso: " + ex.Message);
            }
            finally
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
            //                 userDialogs.Alert($"No se encontro producto para el código: '{upc}'", "Aviso", "Aceptar");
            //             }
            //         }
            //         catch (Exception ex)
            //         {
            //             userDialogs.Alert($"Ha ocurrido un error en el proceso:{ex.Message}", "Alerta", "Aceptar");
            //         }
            //         
            //     }
            // };
            // GoogleVisionBarCodeScanner.Methods.SetIsScanning(true);
            // return;
        }

        private void AddOrUpdateProductoInOden(Producto producto)
        {
            var linea = Orden.Lineas.FirstOrDefault(x => x.ProdutoId == producto.ProductoId);
            if(linea == null)
            {
                if(producto.Existencia <= 0)
                {
                    userDialogs.Alert("Este producto no tiene existencía, no puede ser agregado a la orden.", "Aceptar");
                    return;
                }
                linea = new OrdenDetalle
                {
                    ProdutoId = producto.ProductoId,
                    Upc = producto.Upc,
                    Descripcion = producto.Nombre,
                    Cantidad = 1,
                    Precio = producto.Precio,
                    Existencia = producto.Existencia
                };
                Orden.Lineas.Add(linea);
            } else
            {
                if (producto.Existencia < (linea.Cantidad + 1))
                {
                    userDialogs.Alert("Este producto no tiene existencía suficiente, no puede seguir adicionando a la orden.", "Aceptar");
                    return;
                }
                linea.Cantidad = linea.Cantidad + 1;
            }
            linea.SetDirty();
            Orden.SetDirty();
        }
    }
}
