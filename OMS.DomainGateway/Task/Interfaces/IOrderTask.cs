using OMS.DomainObjects.Request;
using OMS.DomainObjects.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMS.DomainGateway.Task.Interfaces
{
    public interface IOrderTask
    {
        OrderResponse GetOrders(DownloadOrderRequest order);
        OrderResponse CreateUpdateOrders(OrderRequest request);
        OrderResponse CreateOrdersFromShipmentListOCR(OrderRequest request);
        OrderResponse SyncOrders(OrderRequest request);
        OrderStatusCodesResponse GetAllOrderStatus();
        PackingSheetResponse CreateUpdatePackingSheet(PackingSheetRequest packingSheetRequest);
        OrderStatusResponse UpdateOrderStatus(OrderStatusRequest request);
        TripResponse ReAssignTrip(TripRequest tripRequest);
        OrderStatusResponse CancelOrder(OrderStatusRequest request);
    }
}
