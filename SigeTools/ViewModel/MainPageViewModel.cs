using System;
using Xamarin.Essentials;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;
using System.Windows.Input;
using Acr.UserDialogs;
using System.Threading.Tasks;
using SigeMobile.Model;
using SigeTools.Helpers;
using SigeTools.Services;

namespace SigeTools.ViewModel
{
    public class MainPageViewModel : ObservableObject
    {
        public ICommand ShowPedidoCommand
        {
            get
            {
                return new Command(async () =>
                {
                    await Application.Current.MainPage.Navigation.PushAsync(new View.TomaOrdenPage());
                });
            }
        }

        public ICommand ShowConsultaCommand
        {
            get
            {
                return new Command(async () =>
                {
                    await Application.Current.MainPage.Navigation.PushAsync(new View.ConsultaInventarioPage());
                });
            }
        }

        public ICommand ShowConteoCommand
        {
            get
            {
                return new Command(async () =>
                {
                    await Application.Current.MainPage.Navigation.PushAsync(new View.TomaInventarioPage());
                });
            }
        }
        public ICommand ShowInventariosCommand
        {
            get
            {
                return new Command(async () =>
                {
                    await Application.Current.MainPage.Navigation.PushAsync(new View.InventariosPage());
                });
            }
        }
        
        public ICommand LogoutCommand { get; set; } 

        

        private readonly IUserDialogs _dlg;
   
        public MainPageViewModel()
        {

            
            LogoutCommand = new Command(DoLogout);
            _dlg = Acr.UserDialogs.UserDialogs.Instance;

            Usuario = Helpers.Settings.CurrentUser;
            ShowReportCommand = new Command(NavigateReport);
            sigeService = ServiceFactory.SigeService;
            Load();
        }
        ISigeService sigeService;
        private CompanyInfo _companyInfo;
        private bool _showProduct;
        private bool _showPedido;
        private bool _showConteo;
        private bool _showReport;
        private bool _showInventory;

        async void Load()
        {
            try
            {

                ShowProduct = Settings.UserProfile.Contains("1");
                ShowPedido = Settings.UserProfile.Contains("2");
                ShowConteo = Settings.UserProfile.Contains("3");
                ShowReport = Settings.UserProfile.Contains("5");
                ShowInventory = Settings.UserProfile.Contains("4");
                CompanyInfo = await sigeService.GetCompanyInfo();
                
            }
            catch (Exception e)
            {
              await  App.Current.MainPage.DisplayAlert("Error", e.Message, "ok");
            }
        }

        public CompanyInfo CompanyInfo
        {
            get => _companyInfo;
            set
            {
                if (Equals(value, _companyInfo)) return;
                _companyInfo = value;
                OnPropertyChanged();
            }
        }


        private async void NavigateReport()
        {
            await Application.Current.MainPage.Navigation.PushAsync(new View.ReportsPage());
        }

        public string Usuario { get; private set; }
        public ICommand ShowReportCommand { get; }

        public bool ShowProduct
        {
            get => _showProduct;
            set
            {
                if (value == _showProduct) return;
                _showProduct = value;
                OnPropertyChanged();
            }
        }

        public bool ShowPedido
        {
            get => _showPedido;
            set
            {
                if (value == _showPedido) return;
                _showPedido = value;
                OnPropertyChanged();
            }
        }

        public bool ShowConteo
        {
            get => _showConteo;
            set
            {
                if (value == _showConteo) return;
                _showConteo = value;
                OnPropertyChanged();
            }
        }

        public bool ShowReport
        {
            get => _showReport;
            set
            {
                if (value == _showReport) return;
                _showReport = value;
                OnPropertyChanged();
            }
        }

        public bool ShowInventory
        {
            get => _showInventory;
            set
            {
                if (value == _showInventory) return;
                _showInventory = value;
                OnPropertyChanged();
            }
        }


        private async void DoLogout(object obj)
        {
            try
            {
                if (await _dlg.ConfirmAsync("Seguro que desa salir?", "Confirme", "Si", "No"))
                {
                    Xamarin.Forms.Application.Current.MainPage = new View.LoginPage();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
            }
        }

    }
}
