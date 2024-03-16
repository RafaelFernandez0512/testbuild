using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using Acr.UserDialogs;
using SigeMobile.Model;
using SigeTools.Services;
using Xamarin.Forms;

namespace SigeTools.ViewModel
{
    public class InventariosViewModel:INotifyPropertyChanged
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

        IUserDialogs userDialogs;
        ISigeService sigeApi;
        private string _searchText;
        private ObservableCollection<Producto> _productos;
        public ICommand LoadDataCommand { get; set; }
        public ICommand TapItemCommand { get; set; }

        public InventariosViewModel()
        {
            sigeApi = ServiceFactory.SigeService;
            userDialogs = Acr.UserDialogs.UserDialogs.Instance;
            LoadDataCommand = new Command(async () => await LoadData());
            TapItemCommand = new Command<Producto>(TapItem);
            LoadDataCommand.Execute(null);
        }

        private async void TapItem(Producto obj)
        {
            await Application.Current.MainPage.Navigation.PushAsync(new View.ConsultaInventarioPage(obj));
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
                AllProducts = await sigeApi.GetProductInventory();
                Productos =
                    new ObservableCollection<Producto>(AllProducts);
                IsBusy = false;
            }
            catch (Exception ex)
            {
                IsBusy = false;
                userDialogs.Toast("Error en proceso: " + ex.Message);
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