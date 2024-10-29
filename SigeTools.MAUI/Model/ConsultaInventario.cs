using System.Collections.Generic;
using System.Linq;
using PropertyChanged;

namespace SigeTools.MAUI.Model
{
    public class ConsultaInventarioDto
    {
        public string ItemId { get; set; }
        public string ItemIpc { get; set; }
        public string ItemName { get; set; }
        public string StoreId { get; set; }
        public string Location { get; set; }
        public double OnHand { get; set; }
    }
    [AddINotifyPropertyChangedInterface]
    public class ConsultaInventario 
    {
        public Producto Producto { get; set; }
        public List<Existencia> Inventario { get; set; }
        public double Disponible => Inventario != null && Inventario.Any() ? Inventario.Sum(x => x.Disponible) : 0;
    }
}
