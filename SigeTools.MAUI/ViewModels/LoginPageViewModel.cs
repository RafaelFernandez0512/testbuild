using Controls.UserDialogs.Maui;
using Newtonsoft.Json;
using SigeTools.MAUI.Helpers;
using SigeTools.MAUI.Model;
using SigeTools.MAUI.Services;

namespace SigeTools.MAUI.ViewModels
{
    public class LoginPageViewModel : BaseViewModel
    {
        ISigeService sigeService;
        private readonly IUserDialogs _dlg;
        public AsyncDelegateCommand ConfigureCommand { get; set; }

        public CompanyInfo CompanyInfo
        {
            get => _companyInfo;
            set
            {
                if (Equals(value, _companyInfo)) return;
                _companyInfo = value;
            }
        }

        public LoginPageViewModel(INavigationService navigationService,ISigeService sigeService,IUserDialogs userDialogs,IPageDialogService dialogService) : base(navigationService,dialogService)
        {
            this.sigeService =sigeService;
            _dlg = userDialogs;
            Pin = String.Empty;
            ShowsError = false;
            ConfigureCommand = new AsyncDelegateCommand(DoShowConfigure);

        }

        public override async Task InitializeAsync(INavigationParameters parameters)
        {
            //await Load();
        }

        async Task Load()
        {
            try
            {
                CompanyInfo = await sigeService.GetCompanyInfo();
                AppSettings.CompanyInfo = JsonConvert.SerializeObject(CompanyInfo);

            }
            catch (Exception e)
            {
                await PageDialogService.DisplayAlertAsync("Error", e.Message, "ok");
            }
        }

       private async Task DoShowConfigure()
       {
           var url = await PageDialogService.DisplayPromptAsync(AppSettings.BaseServiceUrl, "Configuración", "Aceptar",
               "Cancelar", "Url del Servidor");

           if (!string.IsNullOrEmpty(url))
           {
               AppSettings.BaseServiceUrl =url;
           }
       }

       private bool _showsError;
        public Boolean ShowsError { get; set; }
        private string? _pin = "";
        private CompanyInfo _companyInfo;

        public string? Pin
        {
            get => _pin;
            set
            {
                _pin = value;
                if(_pin is {Length: 4})
                {
           
                    TryLogin(_pin);
                }
            }
        }

        public async void TryLogin(string? pin)
        {
            try
            {
                IsBusy = true;
                IsNotBusy = false;
                //
                (bool loginOk, Usuario usuario) loginResult = await sigeService.IsValidPin(pin);
                if (loginResult.loginOk)
                {
                    //PIN is correct. do whatever you want
                    AppSettings.CurrentUser = loginResult.usuario.UserName;
                    AppSettings.CurrentStore = loginResult.usuario.StoreId;
                    AppSettings.UserProfile = $"{loginResult.usuario.UserPerfil}";
                    await NavigationService.NavigateAsync(UriPages.MainPage);
                }
                else
                {
                    ShowsError = true;
                }

            }
            catch (Exception ex)

            {
                //
            }
            finally
            {
                IsBusy = false;
                IsNotBusy = true;
            }

        }

    }
}
