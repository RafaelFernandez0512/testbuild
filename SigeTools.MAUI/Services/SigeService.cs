using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SigeTools.MAUI.Model;
using SigeTools.MAUI.Model;

namespace SigeTools.MAUI.Services
{
    public class SigeService : ISigeService
    {
        ISigeApi sigeApi;
        public SigeService(ISigeApi sigeApi)
        {
            this.sigeApi = sigeApi;
            _pdfService = new PdfService();
        }

 
        private readonly PdfService _pdfService;



        public async Task<(bool,Usuario)> IsValidPin(string? pin)
        {
            if (String.IsNullOrEmpty(pin) || pin.Length != 4)
            {
                return (false, null);
            } else
            {
                for (var i = 0; i < 4; ++i)
                {
                    if (!Char.IsNumber(pin[i]))
                    {
                        return (false, null);
                    }
                }
            }

            Usuario usuario = await sigeApi.LoginAsync(pin);
            return (usuario != null, usuario);
        }

        public async Task<ConsultaInventario> GetConsultaInventario(string upc)
        {
            ConsultaInventario result = null;

            var dto = await sigeApi.InventarioAsync(upc);
            

            if(dto != null && dto.Any())
            {
                var first = dto.First();
                var itemDto = await sigeApi.GetProductoAsync(first.ItemId);

                var prod = new Producto
                {
                    ProductoId = itemDto.ItemId,
                    Nombre = itemDto.ItemName,
                    Precio = itemDto.ItemPrice,
                    Referencia = itemDto.ItemRef,
                    Upc = upc,
                    Existencia = dto.Sum(x => x.OnHand)
                };
                result = new ConsultaInventario { Producto = prod, Inventario = dto.Select(d => new Existencia { Almacen = d.StoreId, Disponible = d.OnHand, ProductoId = d.ItemId, Ubicacion = d.Location }).ToList() };
            }
            
            return result;
        }

        public  Task<CompanyInfo> GetCompanyInfo()
        {
            try
            {
               return sigeApi.GetCompanyInfoAsync();
            }
            catch (Exception e)
            {
                return null;
            }
          
        }

        public async Task<Producto> GetProducto(string upc)
        {

            Producto result = null;

            var dto = await sigeApi.InventarioAsync(upc);

            if (dto != null && dto.Any())
            {
                var existencia = dto.Where(x => x.StoreId == Helpers.AppSettings.CurrentStore).Sum(x => x.OnHand);
                var first = dto.First();
                var itemDto = await sigeApi.GetProductoAsync(first.ItemId);
                result = new Producto
                {
                    ProductoId = itemDto.ItemId,
                    Nombre = itemDto.ItemName,
                    Precio = itemDto.ItemPrice,
                    Referencia = itemDto.ItemRef,
                    Upc = upc,
                    Existencia = existencia
                };
            }

            return result;
        }

        public async Task<List<Producto>> GetProductInventory()
        {
            var dto = await sigeApi.GetInventoriesHand(Helpers.AppSettings.CurrentStore);
            return dto.Select(x=> new Producto
            {
                ProductoId = x.ItemId,
                Nombre = x.ItemName,
                Precio = x.ItemPrice,
                Referencia = x.ItemRef,
                Upc = x.ItemUpc,
                Existencia = x.OnHand
            }).ToList();
        }

        public async Task<String> SaveOrden(Orden orden, string customerName)
        {
            if (string.IsNullOrEmpty( orden.Transtype))
            {
                orden.Transtype = "CASH";
            }
            var order = await sigeApi.PostOrdenCashAsync( orden.Transtype, customerName, "_", DateTime.Now.Date, Helpers.AppSettings.CurrentUser);
            double orderId = 0;
            Double.TryParse(order, out orderId);
            double seq = 0;
            foreach (var item in orden.Lineas)
            {
                var dto = new OrdenDetalleSync
                {
                    OrderId = orderId,
                    OrderSec = ++seq,
                    ItemId = item.ProdutoId,
                    ItemName = item.Descripcion,
                    SalesQty = item.Cantidad,
                    SalesPrice = item.Precio,
                    Tech = Helpers.AppSettings.CurrentUser,
                };
                await sigeApi.PostOrdenDetalleAsync(dto);
            }
            return order ?? String.Empty;
        }

        public Task<bool> CreateInventoryCount(string locationId,string upc,double qty)
        {
           return  sigeApi.CreateInventoryCount(locationId,upc,qty);
        }

        public byte[] QuotePdf(Orden orden)
        {
            return _pdfService.GenerateQuotePdf(orden);
        }
    }
}
