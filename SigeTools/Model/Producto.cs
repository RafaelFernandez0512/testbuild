using System;

namespace SigeMobile.Model
{
    public class ProductoDto
    {
        public string ItemId { get; set; }
        public string ItemName { get; set; }
        public string ItemFamily { get; set; }
        public string ItemRef { get; set; }
        public string ItemTax { get; set; }
        public double ItemPrice { get; set; }
        public double ItemTime { get; set; }
        public double ItemDiscMax { get; set; }
        public string ItemUpc { get; set; }
        public double OnHand { get; set; }
    }

    public class Producto
    {
        public string ProductoId { get; set; }
        public string Nombre { get; set; }
        public string Upc { get; set; }
        public Double Precio { get; set; }
        public Double Existencia { get; set; }
        public String Referencia { get; set; }
        
    }
}
