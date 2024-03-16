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
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;

namespace SigeTools.ViewModel
{
    public class ReportPageViewModel : INotifyPropertyChanged
    {
        ISigeApi sigeApi;

        private int reportId = 0;
        private readonly IUserDialogs userDialogs;
        private ObservableCollection<object> _reports;

        public List<ColumnText> ColumnTexts
        {
            get => _columnTexts;
            set
            {
                if (Equals(value, _columnTexts)) return;
                _columnTexts = value;
                OnPropertyChanged();
            }
        }

        public List<ColumnText> ColumnTotalsTexts
        {
            get => _columnTotalsTexts;
            set
            {
                if (Equals(value, _columnTotalsTexts)) return;
                _columnTotalsTexts = value;
                OnPropertyChanged();
            }
        }

        private string _columDefinitionsString;
        private List<ColumnText> _columnTexts;
        private double _total;
        private double _totalSelectedReport;
        private object _selectedItem;
        private string _percentage;
        private List<MonthPicker> _months;
        private MonthPicker _selectMonths;
        private bool _showPicker;
        private bool _showPercentage;
        private List<ColumnText> _columnTotalsTexts;
        private bool _isBusy;
        public ICommand LoadDataCommand { get; set; }

        public ObservableCollection<object> Reports
        {
            get => _reports;
            set
            {
                if (Equals(value, _reports)) return;
                _reports = value;
                OnPropertyChanged();
            }
        }

        public double TotalSelectedReport
        {
            get => _totalSelectedReport;
            set
            {
                if (value.Equals(_totalSelectedReport)) return;
                _totalSelectedReport = value;
                OnPropertyChanged();
            }
        }

        public double Total
        {
            get => _total;
            set
            {
                if (value.Equals(_total)) return;
                _total = value;
                OnPropertyChanged();
            }
        }

        public object SelectedItem
        {
            get => _selectedItem;
            set
            {
                if (Equals(value, _selectedItem)) return;
                _selectedItem = value;
                TapItemCommand.Execute(_selectedItem);
                OnPropertyChanged();
            }
        }

        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                if (value == _isBusy) return;
                _isBusy = value;
                OnPropertyChanged();
            }
        }

        public string Percentage
        {
            get => _percentage;
            set
            {
                if (value == _percentage) return;
                _percentage = value;
                OnPropertyChanged();
            }
        }

        public ICommand TapItemCommand { get; set; }

        public List<MonthPicker> Months
        {
            get => _months;
            set
            {
                if (Equals(value, _months)) return;
                _months = value;
                OnPropertyChanged();
            }
        }

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
                OnPropertyChanged();
            }
        }

        public bool ShowPicker
        {
            get => _showPicker;
            set
            {
                if (value == _showPicker) return;
                _showPicker = value;
                OnPropertyChanged();
            }
        }

        public ReportPageViewModel()
        {
            sigeApi = ServiceFactory.SigeApi;
            userDialogs = Acr.UserDialogs.UserDialogs.Instance;
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
                        var salesDay = await sigeApi.GetSalesDayReport();
                        objects.AddRange(salesDay);
                        Total = salesDay.Sum(x => x.Total);
                        ColumnTexts = GenerateColumnTexts("Día", "Efectivo", "Otros", "Total");
                        ColumnTotalsTexts = GenerateColumnTexts("Total", $"${Total:#,0}");
                        break;

                    case 2:
                        var salesMonth = await sigeApi.GetSalesMonthReport();
                        objects.AddRange(salesMonth);
                        Total = salesMonth.Sum(x => x.Total);
                        ColumnTexts = GenerateColumnTexts("Mes", "Total");
                        ColumnTotalsTexts = GenerateColumnTexts("Total", $"${Total:#,0}");
                        break;

                    case 3:
                        var groups = await sigeApi.GetSalesGroupReport();
                        objects.AddRange(groups);
                        Total = groups.Sum(x => x.Total);
                        ColumnTexts = GenerateColumnTexts("Grupo", "Cantidad", "Total");
                        ColumnTotalsTexts = GenerateColumnTexts("Total", $"${Total:#,0}");
                        
                        break;

                    case 4:
                        var balance = await sigeApi.GetCustBalanceReport("1", "A", "Z", DateTime.Now);
                        objects.AddRange(balance);
                        Total = balance.Sum(x => x.Balance);
                        ColumnTexts = GenerateColumnTexts("Cliente", "Total");
                        ColumnTotalsTexts = GenerateColumnTexts("Total", $"${Total:#,0}");
                        break;

                    case 6:
                        var salesProfit = await sigeApi.GetSalesProfit();
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
                        var inventoryHand = await sigeApi.GetInventoryHand();
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
                userDialogs.Toast(e.Message);
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
                OnPropertyChanged();
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