using System;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;
using System.Windows.Input;
using System.Collections.Generic;
using Acr.UserDialogs;
using SigeTools.Services;
using SigeMobile.Model;
using System.Linq;
using System.Threading.Tasks;

namespace SigeTools.ViewModel
{
    public class TomaInventarioPageViewModel : ObservableObject
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

        IUserDialogs userDialogs;
        private string _locationId;
        private readonly IUserDialogs _dlg;

        public TomaInventarioPageViewModel()
        {
            Orden = new Orden();
            //
            Almacen = Helpers.Settings.CurrentStore;
            UseTorch = false;
            UseVibration = true;
            sigeService = ServiceFactory.SigeService;
            ScanDetectedCommand = new Command(DoScanDetected);
            _dlg = Acr.UserDialogs.UserDialogs.Instance;
            IsScanning = true;
            ContinueScanningCommand = new Command(SetScanMode);
            ToogleScannerCommand = new Command(() =>
            {
                ShowScanner = !ShowScanner;
                OnPropertyChanged(nameof(ShowScanner));
                GoogleVisionBarCodeScanner.Methods.SetIsScanning(ShowScanner);
            });
            SearchCommand = new Command(DoSearch);
            SendLineCommand = new AsyncCommand(SendLine);
            userDialogs = Acr.UserDialogs.UserDialogs.Instance;
        }

        void SetScanMode()
        {
            IsScanning = true;
            GoogleVisionBarCodeScanner.Methods.SetIsScanning(true);
            OnPropertyChanged(nameof(IsScanning));
            OnPropertyChanged(nameof(Orden));
        }

        void SetConsultaMode()
        {
            IsScanning = false;
            GoogleVisionBarCodeScanner.Methods.SetIsScanning(false);
            OnPropertyChanged(nameof(IsScanning));
            OnPropertyChanged(nameof(Orden));
        }

        public string UpcText { get; set; }

        public string LocationId
        {
            get => _locationId;
            set
            {
                if (value == _locationId) return;
                _locationId = value;
                OnPropertyChanged();
            }
        }

        private async void DoSearch()
        {
            if (IsBusy) return;

            if (String.IsNullOrEmpty(UpcText))
            {
                userDialogs.Toast("Nada que buscar");
                return;
            }


            try
            {
                SetBusy(true);

                var upc = UpcText;
                var producto = await sigeService.GetProducto(upc);

                SetBusy(false);

                UpcText = String.Empty;
                OnPropertyChanged(nameof(UpcText));

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
                userDialogs.Toast("Eror en proceso: " + ex.Message);
            }
            finally
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
                    try
                    {
                        var upc = scanResults.First().DisplayValue;

                        var producto = await sigeService.GetProducto(upc);

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
                        userDialogs.Alert($"Ha ocurrido un error en el proceso:{ex.Message}", "Alerta", "Aceptar");
                    }
                }
            };
            GoogleVisionBarCodeScanner.Methods.SetIsScanning(true);
            return;
        }

       async Task SendLine()
       {
           try
           {
               if(!await userDialogs.ConfirmAsync(new ConfirmConfig { Title = "Confirme", Message = "Seguro que desea enviar el conteo de inventario?", OkText = "Enviar", CancelText = "Cancelar" }))
               {
                   return;
               }
               await  Device.InvokeOnMainThreadAsync(() => _dlg.ShowLoading());
               var lines = Orden.Lineas.ToList();
                var linesSend = new List<OrdenDetalle>();
               foreach (var line in lines)
               {
                  
                   var  hasSend = await sigeService.CreateInventoryCount(LocationId,line.Upc,line.Cantidad);
                   if (hasSend)
                   {
                       linesSend.Add(line);
                   
                   }
               }

               foreach (var line in linesSend)
               {
                   Orden.Lineas.Remove(line);
               }
          
               await  Device.InvokeOnMainThreadAsync(() => _dlg.HideLoading());
           }
           catch (Exception e)
           {
             await  Device.InvokeOnMainThreadAsync(() => _dlg.HideLoading());
             await  App.Current.MainPage.DisplayAlert("Error", e.Message, "ok");
             
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
