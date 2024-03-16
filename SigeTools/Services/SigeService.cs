using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SigeMobile.Model;

namespace SigeTools.Services
{
    public class SigeService : ISigeService
    {
        ISigeApi sigeApi;
        public SigeService()
        {
            sigeApi = ServiceFactory.SigeApi;
            _pdfService = new PdfService();
        }

        readonly List<Usuario> usuarios = new List<Usuario>();
        private Task GetUsuarios()
        {
            if(!usuarios.Any())
            {
                usuarios.Add(new Usuario { UserId = "user1", UserPassword = "1111", UserName = "Usuario 1" });
                usuarios.Add(new Usuario { UserId = "user2", UserPassword = "2222", UserName = "Usuario 2" });
                usuarios.Add(new Usuario { UserId = "user3", UserPassword = "3333", UserName = "Usuario 3" });
            }
            return Task.CompletedTask;
        }

        readonly List<Producto> productos = new List<Producto>();
        private Task GetProductos()
        {
            if (!productos.Any())
            {
                productos.Add(new Producto { ProductoId = "P-01", Nombre = "Producto ABC", Precio = 300, Upc = "123456789" } );
                productos.Add(new Producto { ProductoId = "P-02", Nombre = "Producto DEF", Precio = 400, Upc = "456456123" });
                productos.Add(new Producto { ProductoId = "P-03", Nombre = "Producto GHI", Precio = 200, Upc = "789456123" });
            }
            return Task.CompletedTask;
        }

        readonly List<Existencia> existencias = new List<Existencia>();
        private readonly PdfService _pdfService;

        private Task GetExistencias()
        {
            if (!existencias.Any())
            {
                existencias.Add(new Existencia { ProductoId = "P-01", Almacen = "AP", Disponible = 15.2, Ubicacion = "A-1-01" });
                existencias.Add(new Existencia { ProductoId = "P-01", Almacen = "AP", Disponible = 3, Ubicacion = "A-1-02" });
                existencias.Add(new Existencia { ProductoId = "P-01", Almacen = "SA", Disponible = 10, Ubicacion = "B-1-01" });

                existencias.Add(new Existencia { ProductoId = "P-02", Almacen = "AP", Disponible = 19, Ubicacion = "A-1-01" });
                existencias.Add(new Existencia { ProductoId = "P-02", Almacen = "AP", Disponible = 10, Ubicacion = "A-1-02" });

                existencias.Add(new Existencia { ProductoId = "P-03", Almacen = "AP", Disponible = 4, Ubicacion = "A-1-01" });
                existencias.Add(new Existencia { ProductoId = "P-03", Almacen = "SA", Disponible = 30, Ubicacion = "B-1-01" });
            }
            return Task.CompletedTask;
        }

        public async Task<(bool,Usuario)> IsValidPin(string pin)
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

            //await GetUsuarios();
            //Usuario usuario = usuarios.SingleOrDefault(x => x.UserPassword == pin);

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
                var existencia = dto.Where(x => x.StoreId == Helpers.Settings.CurrentStore).Sum(x => x.OnHand);
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
            var dto = await sigeApi.GetInventoriesHand(Helpers.Settings.CurrentStore);
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
            var order = await sigeApi.PostOrdenCashAsync( orden.Transtype, customerName, "_", DateTime.Now.Date, Helpers.Settings.CurrentUser);
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
                    Tech = Helpers.Settings.CurrentUser,
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
