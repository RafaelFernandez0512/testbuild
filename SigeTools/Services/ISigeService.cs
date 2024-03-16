using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SigeMobile.Model;

namespace SigeTools.Services
{
    public interface ISigeService
    {
        Task<(Boolean,Usuario)> IsValidPin(string pinId);
        Task<ConsultaInventario> GetConsultaInventario(string upc);
        Task<CompanyInfo> GetCompanyInfo();
        Task<Producto> GetProducto(string upc);
        Task<List<Producto>> GetProductInventory();
        Task<String> SaveOrden(Orden orden, string customerName);
        Task<bool> CreateInventoryCount(string locationId, string upc, double qty);
        byte[] QuotePdf(Orden orden);
    }
}
