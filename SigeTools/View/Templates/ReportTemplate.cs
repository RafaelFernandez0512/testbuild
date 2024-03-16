using SigeMobile.Model;
using Xamarin.Forms;

namespace SigeTools.View.Templates
{
    public class ReportDataTemplateSelector:DataTemplateSelector
    {
        public DataTemplate SalesDayReportTemplate { get; set; }
        public DataTemplate SalesMonthReportTemplate { get; set; }
        public DataTemplate SalesGroupReportTemplate { get; set; }
        public DataTemplate CustBalanceReportTemplate { get; set; }
        
        public DataTemplate InventoryOnHandReportTemplate { get; set; }
        
        public DataTemplate SalesProfitReportTemplate { get; set; }
        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            switch (item)
            {
                case SalesDayReport salesDayReport:
                    return SalesDayReportTemplate;
                case SalesMonthReport salesMonth:
                    return SalesMonthReportTemplate;
                case SalesGroupReport salesGroupReport:
                    return SalesGroupReportTemplate;
                case CustBalanceReport custBalanceReport:
                    return CustBalanceReportTemplate;
                case SalesProfitReport salesProfitReport:
                    return SalesProfitReportTemplate;
                case InventoryOnHandReport inventoryOnHandReport:
                    return InventoryOnHandReportTemplate;
                default:
                    return null;
                    
                  
            }
        }
    }
}