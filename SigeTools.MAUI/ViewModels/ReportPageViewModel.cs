using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Controls.UserDialogs.Maui;
using SigeTools.MAUI.Model;
using SigeTools.MAUI.Services;
using SigeTools.ViewModel;

namespace SigeTools.MAUI.ViewModels
{
    public class ReportPageViewModel : BaseViewModel
    {
        ISigeApi _sigeApi;

        private int reportId = 0;
        private readonly IUserDialogs userDialogs;
        private ObservableCollection<object> _reports;

        public List<ColumnText> ColumnTexts{ get; set; }

        public List<ColumnText> ColumnTotalsTexts { get; set; }
    

        private object _selectedItem;

        private List<MonthPicker> _months;
        private MonthPicker _selectMonths;
        private bool _showPicker;
        private bool _showPercentage;
        public ICommand LoadDataCommand { get; set; }

        public ObservableCollection<object> Reports{ get; set; }

        public double TotalSelectedReport{ get; set; }

        public double Total{ get; set; }

        public object SelectedItem
        {
            get => _selectedItem;
            set
            {
                if (Equals(value, _selectedItem)) return;
                _selectedItem = value;
                TapItemCommand.Execute(_selectedItem);
            }
        }


        public string Percentage { get; set; }

        public ICommand TapItemCommand { get; set; }

        public List<MonthPicker> Months { get; set; }

        public MonthPicker SelectMonths
        {
            get => _selectMonths;
            set
            {
                if (value == _selectMonths) return;
                _selectMonths = value;
                Reports = new ObservableCollection<object>(AllItemsReport.Select(x=> x as SalesProfitReport).Where(x=>x.MonthNum == _selectMonths.SalesNum));
                SelectedItem = Reports.FirstOrDefault();
                var total = Reports.Select(x => x as SalesProfitReport).Sum(x => x.CostTotal);
                Total = Reports.Select(x=> x as SalesProfitReport).Sum(x => x.SalesTotal);
                var salesProfit = Reports.Select(x=> x as SalesProfitReport).Sum(x => x.SalesProfit);
                ColumnTotalsTexts = GenerateColumnTexts("Totales", $"${total:#,0}",$"${Total:#,0}",$"${salesProfit:#,0}");
            }
        }

        public bool ShowPicker { get; set; }

        public ReportPageViewModel(INavigationService navigationService, IPageDialogService dialogService,IUserDialogs userDialogs,ISigeApi sigeApi) : base(navigationService,dialogService)
        {
            _sigeApi = sigeApi;
           this. userDialogs =userDialogs;
            LoadDataCommand = new Command(async () => await LoadData());
            TapItemCommand = new Command(TapItem);
        }

        private void TapItem(object obj)
        {
            Percentage = "";
            switch (obj)
            {
                case SalesDayReport dayReport:
                    TotalSelectedReport = dayReport.Total;
                    break;
                case SalesMonthReport monthReport:
                    TotalSelectedReport = monthReport.Total;
                    break;
                case SalesGroupReport groupReport:
                    TotalSelectedReport = groupReport.Total;
                    break;
                case CustBalanceReport custBalanceReport:
                    TotalSelectedReport = custBalanceReport.Balance;
                    break;
                case InventoryOnHandReport inventoryOnHandReport:
                    TotalSelectedReport = inventoryOnHandReport.Total;
                    break;
                case SalesProfitReport salesProfit:
                    TotalSelectedReport = salesProfit.SalesProfit;
                    Percentage = $"{salesProfit.MarginPct/100:P2}";
                    break;
                default:
                    TotalSelectedReport = 0;
                    break;
            }
        }

        public void Init(object parameter)
        {
            reportId = (int) parameter;
        }

        async Task LoadData()
        {
            try
            {
                IsBusy = true;
                List<object> objects = new List<object>();

                switch (reportId)
                {
                    case 1:
                        var salesDay = await _sigeApi.GetSalesDayReport();
                        objects.AddRange(salesDay);
                        Total = salesDay.Sum(x => x.Total);
                        ColumnTexts = GenerateColumnTexts("Día", "Efectivo", "Otros", "Total");
                        ColumnTotalsTexts = GenerateColumnTexts("Total", $"${Total:#,0}");
                        break;

                    case 2:
                        var salesMonth = await _sigeApi.GetSalesMonthReport();
                        objects.AddRange(salesMonth);
                        Total = salesMonth.Sum(x => x.Total);
                        ColumnTexts = GenerateColumnTexts("Mes", "Total");
                        ColumnTotalsTexts = GenerateColumnTexts("Total", $"${Total:#,0}");
                        break;

                    case 3:
                        var groups = await _sigeApi.GetSalesGroupReport();
                        objects.AddRange(groups);
                        Total = groups.Sum(x => x.Total);
                        ColumnTexts = GenerateColumnTexts("Grupo", "Cantidad", "Total");
                        ColumnTotalsTexts = GenerateColumnTexts("Total", $"${Total:#,0}");
                        
                        break;

                    case 4:
                        var balance = await _sigeApi.GetCustBalanceReport("1", "A", "Z", DateTime.Now);
                        objects.AddRange(balance);
                        Total = balance.Sum(x => x.Balance);
                        ColumnTexts = GenerateColumnTexts("Cliente", "Total");
                        ColumnTotalsTexts = GenerateColumnTexts("Total", $"${Total:#,0}");
                        break;

                    case 6:
                        var salesProfit = await _sigeApi.GetSalesProfit();
                        objects.AddRange(salesProfit);
                        Months = salesProfit.GroupBy(x => x.MonthNum).Select(x => new MonthPicker()
                        {
                            SalesMonth = x.FirstOrDefault()?.SalesMonth,
                            SalesNum = x.FirstOrDefault()?.MonthNum
                        }).ToList();
                        ColumnTexts = GenerateColumnTexts("Area/Grupo", "Costos", "Ventas", "Beneficio");
                       
                        ShowPicker = true;
                        ShowPercentage = true;
                        break;

                    case 5:
                        var inventoryHand = await _sigeApi.GetInventoryHand();
                        objects.AddRange(inventoryHand);
                        Total = inventoryHand.Sum(x => x.Total);
                        ColumnTexts = GenerateColumnTexts("Almacen", "Grupo", "Cantidad", "Total");
                        ColumnTotalsTexts = GenerateColumnTexts("Total", $"{Total:#,0}");
                        break;
                }
                
                IsBusy = false;
                if (ShowPicker)
                {
                    AllItemsReport = new List<object>(objects);
                    SelectMonths = Months.FirstOrDefault(x => x.SalesMonth == $"{DateTime.Now:yyyy-MM}")?? Months.LastOrDefault();
                }
                else
                {
                    Reports = new ObservableCollection<object>(objects);
                    SelectedItem = Reports.FirstOrDefault();
                }
              
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                userDialogs.ShowToast(e.Message);
                IsBusy = false;
            }
            finally
            {
                // userDialogs.HideLoading();
                IsBusy = false;
            }
        }

        public bool ShowPercentage
        {
            get => _showPercentage;
            set
            {
                if (value == _showPercentage) return;
                _showPercentage = value;
            }
        }

        public List<object> AllItemsReport { get; set; }

        List<ColumnText> GenerateColumnTexts(params string[] texts)
        {
            var columnTexts = new List<ColumnText>();

            for (int i = 0; i < texts.Length; i++)
            {
                var columnText = new ColumnText()
                {
                    Column = i,
                    Text = texts[i],
                };
                if (i >0 )
                {
                    columnText.HorizontalLayoutOptions = LayoutOptions.Center;
                }

                if (i == texts.Length-1 )
                {
                    columnText.HorizontalLayoutOptions = LayoutOptions.End;
                }

                columnTexts.Add(columnText);
            }

            return columnTexts;
        }

    }

    public class MonthPicker
    {
        public string SalesMonth { get; set; }
        public string SalesNum { get; set; }
        public override string ToString()
        {
            return SalesMonth;
        }
    }
}