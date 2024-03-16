using System;
using System.Linq;
using Xamarin.CommunityToolkit.ObjectModel;

namespace SigeMobile.Model
{
    public class Orden : ObservableObject
    {
        public Orden()
        {
            Lineas = new ObservableRangeCollection<OrdenDetalle>();
        }
        public string OrdenId { get; set; }
        public ObservableRangeCollection<OrdenDetalle> Lineas { get; set; }
        public double Total => Lineas != null & Lineas.Any() ? Lineas.Sum(x => x.Total) : 0;
        public int SkuCount => Lineas != null ? Lineas.Count : 0;
        public double Itbis { get; set; }
        public string CustomerName { get; set; }
        public string Transtype { get; set; }

        public void SetDirty()
        {
            OnPropertyChanged(nameof(Total));
            OnPropertyChanged(nameof(SkuCount));
        }
    }

    public class OrdenDetalle : ObservableObject
    {
        public string Upc { get; set; }
        public string ProdutoId { get; set; }
        public string Descripcion { get; set; }
        public double Existencia { get; set; }
        double cantidad;
        public double Cantidad
        {
            get { return cantidad; }
            set
            {
                cantidad = value;
                SetDirty();
            }
        }
        public double Precio { get; set; }
        public double Total { get; set; }
        public void SetDirty()
        {
            Total =  Cantidad * Precio;
            OnPropertyChanged(nameof(Total));
            OnPropertyChanged(nameof(Cantidad));
            OnPropertyChanged(nameof(Total));
        }
    }

    public class OrdenDetalleSync
    {
        public Double OrderId { get; set; }
        public Double OrderSec { get; set; }
        public String ItemId { get; set; }
        public string ItemName { get; set; }
        public string Tech { get; set; }
        public Double SalesQty { get; set; }
        public Double SalesPrice { get; set; }
    }
}
