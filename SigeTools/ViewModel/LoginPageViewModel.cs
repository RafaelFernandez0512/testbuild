using System;
using System.Threading.Tasks;
using Acr.UserDialogs;
using Newtonsoft.Json;
using SigeMobile.Model;
using SigeTools.Helpers;
using SigeTools.Services;
using Xamarin.CommunityToolkit.ObjectModel;

namespace SigeTools.ViewModel
{
    public class LoginPageViewModel : ObservableObject
    {
        ISigeService sigeService;
        private readonly IUserDialogs _dlg;
        public IAsyncCommand ConfigureCommand { get; set; }

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

        public LoginPageViewModel()
        {
            sigeService = ServiceFactory.SigeService;
            _dlg = Acr.UserDialogs.UserDialogs.Instance;
            Pin = String.Empty;
            ShowsError = false;
            ConfigureCommand = new AsyncCommand(DoShowConfigure);
            Load();
        }

       async void Load()
        {
            try
            {
                CompanyInfo = await sigeService.GetCompanyInfo();
                Settings.CompanyInfo = JsonConvert.SerializeObject(CompanyInfo);

            }
            catch (Exception e)
            {
                await  App.Current.MainPage.DisplayAlert("Error", e.Message, "ok");
            }
        }
        private async Task DoShowConfigure()
        {
            var userDlg = UserDialogs.Instance;
            var promptConfig = new PromptConfig
            {
                Text = Settings.BaseServiceUrl,
                Message = "Url del Servidor",
                Title = "Configuración",
                OkText = "Aceptar",
                CancelText = "Cancelar",
                InputType = InputType.Url
            };

            var dlgResult = await userDlg.PromptAsync(promptConfig);

            if (dlgResult.Ok)
            {
                Settings.BaseServiceUrl = dlgResult.Text;
            }
        }

        private bool _showsError;
        public Boolean ShowsError
        {
            get => _showsError;
            set => SetProperty(ref _showsError, value);
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

        private string _pin = "";
        private CompanyInfo _companyInfo;

        public string Pin
        {
            get => _pin;
            set
            {
                SetProperty(ref _pin, value);
                if(value !=  null && value.Length == 4)
                {
                    TryLogin(value);
                }
            }
        }

        public async void TryLogin(string pin)
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
                    Helpers.Settings.CurrentUser = loginResult.usuario.UserName;
                    Helpers.Settings.CurrentStore = loginResult.usuario.StoreId;
                    Helpers.Settings.UserProfile = $"{loginResult.usuario.UserPerfil}";
                    Xamarin.Forms.Application.Current.MainPage = new Xamarin.Forms.NavigationPage(new View.MainPage());
                } else
                {
                    ShowsError = true;
                }

            }
            catch (Exception ex)
            {
                //
            } finally
            {
                IsBusy = false;
                IsNotBusy = true;
            }

        }

    }
}
