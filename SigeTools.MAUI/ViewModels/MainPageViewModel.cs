using Controls.UserDialogs.Maui;
using SigeTools.MAUI.Helpers;
using SigeTools.MAUI.Model;
using SigeTools.MAUI.Services;

namespace SigeTools.MAUI.ViewModels
{
    public class MainPageViewModel : BaseViewModel
    {

        
        public AsyncDelegateCommand LogoutCommand { get; set; } 
        ISigeService sigeService;
        private CompanyInfo _companyInfo;

        public AsyncDelegateCommand ShowPedidoCommand { get; }
        public AsyncDelegateCommand ShowConsultaCommand { get; }
        public AsyncDelegateCommand ShowConteoCommand { get; }
        public AsyncDelegateCommand ShowInventariosCommand { get; }
        public string Usuario { get; private set; }
        public AsyncDelegateCommand ShowReportCommand { get; }

        public bool ShowProduct { get; set; }

        public bool ShowPedido { get; set; }

        public bool ShowConteo { get; set; }
        public bool ShowReport { get; set; }

        public bool ShowInventory{ get; set; }
        

        private readonly IUserDialogs _dlg;
   
        public MainPageViewModel(INavigationService navigationService,ISigeService sigeService,IPageDialogService pageDialogService,IUserDialogs userDialogs) : base(navigationService,pageDialogService)
        {
            LogoutCommand = new AsyncDelegateCommand(DoLogout);
            _dlg = userDialogs;

            Usuario = AppSettings.CurrentUser;
            ShowReportCommand = new AsyncDelegateCommand(NavigateReport);
           this.sigeService = sigeService;
            ShowPedidoCommand = new AsyncDelegateCommand(ExecuteShowPedidoAsync);
            ShowConsultaCommand = new AsyncDelegateCommand(ExecuteShowConsultaAsync);
            ShowConteoCommand = new AsyncDelegateCommand(ExecuteShowConteoAsync);
            ShowInventariosCommand = new AsyncDelegateCommand(ExecuteShowInventariosAsync);
            Load();
        }

        private async Task ExecuteShowPedidoAsync()
        {
            await NavigationService.NavigateAsync("TomaOrdenPage");
        }

        private async Task ExecuteShowConsultaAsync()
        {
            await NavigationService.NavigateAsync("ConsultaInventarioPage");
        }

        private async Task ExecuteShowConteoAsync()
        {
            await NavigationService.NavigateAsync("TomaInventarioPage");
        }

        private async Task ExecuteShowInventariosAsync()
        {
            await NavigationService.NavigateAsync("InventariosPage");
        }
        async void Load()
        {
            try
            {

                ShowProduct = AppSettings.UserProfile.Contains("1");
                ShowPedido = AppSettings.UserProfile.Contains("2");
                ShowConteo = AppSettings.UserProfile.Contains("3");
                ShowReport = AppSettings.UserProfile.Contains("5");
                ShowInventory = AppSettings.UserProfile.Contains("4");
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
            }
        }


        private  Task NavigateReport()
        {
          return  NavigationService.NavigateAsync(UriPages.ReportsPage);
        }




        private async Task DoLogout()
        {
            if (await _dlg.ConfirmAsync("Seguro que desa salir?", "Confirme", "Si", "No"))
            {
             await    NavigationService.NavigateAsync(UriPages.LoginPage);
            }
        }

    }
}
