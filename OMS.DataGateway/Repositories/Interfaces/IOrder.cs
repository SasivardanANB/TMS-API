using OMS.DomainObjects.Request;
using OMS.DomainObjects.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain = OMS.DomainObjects.Objects;

namespace OMS.DataGateway.Repositories.Iterfaces
{
    public interface IOrder
    {
        OrderResponse GetOrders(DownloadOrderRequest orderRequest);
        OrderResponse CreateUpdateOrders(OrderRequest request);
        OrderStatusCodesResponse GetAllOrderStatus();
        OrderResponse SyncOrders(OrderRequest request);
        PackingSheetResponse CreateUpdatePackingSheet(PackingSheetRequest packingSheetRequest);
        OrderStatusResponse UpdateOrderStatus(OrderStatusRequest request);
        TripResponse ReAssignTrip(TripRequest tripRequest);
        OrderStatusResponse CancelOrder(OrderStatusRequest request);
    }
}
