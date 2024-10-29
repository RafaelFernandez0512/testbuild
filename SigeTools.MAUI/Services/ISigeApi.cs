using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Refit;
using SigeTools.MAUI.Model;
using SigeTools.MAUI.Model;

namespace SigeTools.MAUI.Services
{
    public interface ISigeApi
    {
        [Get("/CompanyInfo")]
        Task<CompanyInfo> GetCompanyInfoAsync();
        [Get("/Usuarios")]
        Task<List<Usuario>> GetUsuariosAsync();
        [Get("/Usuarios/{id}")]
        Task<Usuario> GetUsuarioAsync(string id);
        [Post("/Login/ByPin?pin={pin}")]
        Task<Usuario> LoginAsync(string? pin);

        [Get("/Inventario?upc={upc}")]
        Task<List<ConsultaInventarioDto>> InventarioAsync(string upc);

        [Get("/Productos/{id}")]
        Task<ProductoDto> GetProductoAsync(string id);

        [Post("/Ordenes?CustomerId={CustomerId}&DeliveryDate={DeliveryDate}&UserId={UserId}")]
        Task<String> PostOrdenAsync(string CustomerId, DateTime DeliveryDate, String UserId);

        [Post("/OrdenesCash?CustomerId={CustomerId}&CustomerName={CustomerName}&CarName={CarName}&DeliveryDate={DeliveryDate}&UserId={UserId}")]
        Task<String> PostOrdenCashAsync(string CustomerId, string CustomerName, string CarName, DateTime DeliveryDate, String UserId);

        [Post("/OrdenesDetalle")]
        Task<OrdenDetalle> PostOrdenDetalleAsync([Body] OrdenDetalleSync orderDetalle);
        
        
        [Get("/GetSalesDayReport")]
        Task<List<SalesDayReport>> GetSalesDayReport();
                
        [Get("/GetCustBalanceReport?company={company}&formgroup={formgroup}&togroup={togroup}&todate={todate}")]
        Task<List<CustBalanceReport>> GetCustBalanceReport(string company,string formgroup,string togroup,DateTime todate);
                
        [Get("/GetSalesGroupReport")]
        Task<List<SalesGroupReport>> GetSalesGroupReport();
                
        [Get("/GetSalesMonthReport")]
        Task<List<SalesMonthReport>> GetSalesMonthReport();
        [Get("/GetSalesProfit")]
        Task<List<SalesProfitReport>> GetSalesProfit();
        [Get("/GetInventoryHand")]
        Task<List<InventoryOnHandReport>> GetInventoryHand();
        
        [Get("/Inventario/OnHand/All?storeId={currentStore}")]
        Task<List<ProductoDto>> GetInventoriesHand(string currentStore);
        [Post("/Inventario/CreateInventoryCount?locationID={locationId}&itemUPC={upc}&countQty={qty}")]
        Task<bool> CreateInventoryCount(string locationId, string upc, double qty);

        //[Get("/Clientes")]
        //Task<List<Cliente>> GetClientesAsync();
        //[Get("/Clientes/{id}")]
        //Task<Cliente> GetClienteAsync(string id);
        //[Post("/Clientes?userId={userId}")]
        //Task<Cliente> PostClienteAsync([Body] Cliente cliente, string userId);

        //[Get("/Productos")]
        //Task<List<Producto>> GetProductosAsync();

        //[Get("/Lavadores")]
        //Task<List<Lavador>> GetLavadoresAsync();
        //[Get("/Lavadores/{id}")]
        //Task<Cliente> GetLavadorAsync(string id);

        //[Get("/Ordenes")]
        //Task<List<Orden>> GetOrdenesAsync();
        //[Get("/Ordenes/{id}")]
        //Task<Orden> GetOrdenAsync(double id);
        //[Put("/Ordenes?orderId={OrdenId}&status={Status}")]
        //Task PutOrdenStatusAsync(long OrdenId, string Status);

        //[Get("/OrdenesDetalle")]
        //Task<List<OrdenDetalle>> GetOrdenDetalleAsync();
        //[Get("/OrdenesDetalle?orderId={orderId}")]
        //Task<List<OrdenDetalle>> GetOrdenDetalleAsync(double orderId);


        //[Get("/OrdenCheckList/{id}")]
        //Task<List<OrdenCheckList>> GetOrdenCheckListAsync(double id);
        //[Post("/OrdenCheckList")]
        //Task<OrdenCheckList> PostOrdenCheckListAsync([Body] OrdenCheckList ordenCheckList);

        //[Get("/Tables")]
        //Task<List<TableDTO>> GetTablesAsync();
        //[Get("/Tables/{tableId}")]
        //Task<TableDTO> GetTableAsync(string tableId);

        //[Get("/Categorias")]
        //Task<List<MenuCategory>> GetMenuCategoriesAsync();
        //[Get("/Categoryitems")]
        //Task<List<MenuCategoryItem>> GetMenuCategoryItemsAsync();
        //[Get("/items")]
        //Task<List<MenuItem>> GetMenuItemsAsync();
        //[Get("/opciones")]
        //Task<List<MenuItemOption>> GetMenuItemOptionsAsync();

        //[Get("/Comandas/{orderId}")]
        //Task<Comanda> GetComanda(double orderId);
        //[Get("/ComandasDetalle")]
        //Task<List<ComandaDetalle>> GetComandaDetalle(double orderId);
        //[Get("/ComandasDetalle/Options/{transId}")]
        //Task<List<ComandaDetalleOption>> GetComandaDetalleOptions(long transId);

        //[Post("/Comandas?TableId={TableId}&DeliveryDate={DeliveryDate}&UserId={UserId}")]
        //Task<String> PostComandaAsync(string TableId, DateTime DeliveryDate, String UserId);
        //[Post("/Comandas/CreateSplitTable?Company={Company}&TableId={TableId}&UserId={UserId}")]
        //Task PostSplitTableComandaAsync(short Company, string TableId, String UserId);
        //[Put("/Comandas/SetKitchen?orderId={OrderId}")]
        //Task<String> SendComandaToKitchenAsync(double OrderId);
        //[Put("/Comandas/SetTable?orderId={orderId}&TableId={tableId}")]
        //Task SetComandaTableAsync(double orderId, string TableId);
        //[Put("/Comandas/RequestPrint?orderId={OrderId}")]
        //Task<String> SendComandaToPrinterAsync(double OrderId);
        //[Put("/Comandas/SetNombre?orderId={OrderId}&nombre={Nombre}")]
        //Task SetComandaNameAsync(double OrderId, string Nombre);
        //[Post("/Comandas/Split?orderId={OrderId}&splitType={SplitType}")]
        //Task<String> SplitComandaAsync(double OrderId, short SplitType);

        //[Post("/ComandasDetalle")]
        //Task<OrdenDetalleSync> PostComandasDetalleAsync([Body] OrdenDetalleSync orderDetalle);
        //[Delete("/ComandasDetalle?transId={TransId}&UserId={UserId}")]
        //Task<String> DeleteComandaDetalleAsync(long TransId, string UserId);
        //[Put("/ComandasDetalle/SetQty?transId={TransId}&qty={Qty}&plusMinus={PlusMinus}")]
        //Task<String> UpdateComandaDetalleQtyAsync(long TransId, int Qty, short PlusMinus);
        //[Put("/ComandasDetalle/SetSeat?transId={TransId}&seat={Seat}")]
        //Task<String> UpdateComandaDetalleSeatAsync(long TransId, int Seat);
        //[Post("/ComandasDetalle/SetOptions?transId={TransId}&opcionesList={Opciones}&ItemId={ItemId}&textoLibre={Texto}")]

    }
}
