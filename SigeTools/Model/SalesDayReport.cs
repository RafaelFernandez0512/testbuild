using System;

namespace SigeMobile.Model
{
    public class SalesDayReport
    {

        public DateTime SaleDate { get; set; }

        public string Day { get; set; }

        public double Cash { get; set; }

        public double Other { get; set; }
   
        public double Total { get; set; }
    }
}