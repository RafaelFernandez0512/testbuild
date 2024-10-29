using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Controls.UserDialogs.Maui;
using SigeTools.MAUI.Helpers;
using SigeTools.MAUI.Model;
using SigeTools.MAUI.Services;

namespace SigeTools.MAUI.ViewModels
{
    public class InventariosViewModel:BaseViewModel
    {
        public bool IsBusy { get; set; }

        public string SearchText
        {
            get => _searchText;
            set
            {
                if (value == _searchText) return;
                _searchText = value;
                Productos = new ObservableCollection<Producto>(AllProducts
                    .Where(x => string.IsNullOrEmpty(_searchText)|| x.Nombre!=null &&
                    x.Nombre.ToUpper().Contains(_searchText.ToUpper())));
                OnPropertyChanged();
            }
        }

        public List<Producto> AllProducts { get; set; }
        public ObservableCollection<Producto> Productos
        {
            get => _productos;
            set
            {
                if (Equals(value, _productos)) return;
                _productos = value;
  
                OnPropertyChanged();
            }
        }

        IUserDialogs _userDialogs;
        ISigeService _sigeApi;
        private string _searchText;
        private ObservableCollection<Producto> _productos;
        public AsyncDelegateCommand LoadDataCommand { get; set; }
        public AsyncDelegateCommand<Producto> TapItemCommand { get; set; }

        public InventariosViewModel(INavigationService navigationService,IPageDialogService pageDialogService,IUserDialogs userDialogs,ISigeApi sigeApi) : base(navigationService,pageDialogService)
        {
            sigeApi = sigeApi;
            _userDialogs = userDialogs;
            LoadDataCommand = new AsyncDelegateCommand(async () => await LoadData());
            TapItemCommand = new AsyncDelegateCommand<Producto>(TapItem);
            LoadDataCommand.Execute(null);
        }

        private  Task TapItem(Producto obj)
        {
          return  NavigationService.NavigateAsync(UriPages.ConsultaInventarioPage, new NavigationParameters()
            {
                {"Producto", obj},
            });
        }


        async Task LoadData()
        {
            if (IsBusy)
            {
                return;
            }
            IsBusy = true;

            try
            {
                AllProducts = await _sigeApi.GetProductInventory();
                Productos =
                    new ObservableCollection<Producto>(AllProducts);
                IsBusy = false;
            }
            catch (Exception ex)
            {
                IsBusy = false;
                _userDialogs.ShowToast("Error en proceso: " + ex.Message);
            }
            finally
            {
                IsBusy = false;
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}