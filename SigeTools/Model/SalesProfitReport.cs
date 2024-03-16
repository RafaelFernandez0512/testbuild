namespace SigeMobile.Model
{
    public class SalesProfitReport
    {
        public string MonthNum { get; set; }
        public string SalesMonth { get; set; }
        public string PosType { get; set; }
        public double CostTotal { get; set; }
        public double SalesTotal { get; set; }
        public double SalesProfit { get; set; }
        public double MarginPct { get; set; }
    }
}