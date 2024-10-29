using System.Collections.ObjectModel;
using System.Windows.Input;
using SigeTools.MAUI.Helpers;
using SigeTools.MAUI.Model;

namespace SigeTools.MAUI.ViewModels
{
    public class ColumnText
    {
        public int Column { get; set; }
        public string Text { get; set; }
        public LayoutOptions HorizontalLayoutOptions { get; set; } = LayoutOptions.Start;
    }
    public class ReportsViewModel: BaseViewModel
    {
        public ICommand NavigateToReportCommand { get; set; }
        public ObservableCollection<MenuReport> MenuReports { get; set; }

        public ReportsViewModel(INavigationService navigationService,IPageDialogService dialogService):base(navigationService,dialogService)
        {
            Load();
            NavigateToReportCommand = new Command<MenuReport>(NavigateToReport);
        }

        private async void NavigateToReport(MenuReport obj)
        {
           await  NavigationService.NavigateAsync(UriPages.ReportPage, new NavigationParameters()
            {
                { "Report", obj }
            });

        }

        void Load()
        {

            MenuReports = new ObservableCollection<MenuReport>()
            {
                new MenuReport()
                {
                    Id = 1,
                    Title = "Ventas Diarias",
                },
                new MenuReport()
                {
                    Id = 2,
                    Title = "Ventas Mensuales",
                },
                new MenuReport()
                {
                    Id = 3,
                    Title = "Ventas por Grupos",
                },
                new MenuReport()
                {
                    Id = 4,
                    Title = "Cuentas por Cobrar",
                },
                new MenuReport()
                {
                    Id = 5,
                    Title = "Inventario Total",
                },
                new MenuReport()
                {
                    Id = 6,
                    Title = "Márgenes de ventas",
                },
            };
        }
    }
}