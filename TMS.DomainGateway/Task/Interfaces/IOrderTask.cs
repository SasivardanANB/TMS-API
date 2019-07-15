using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMS.DomainObjects.Objects;
using TMS.DomainObjects.Request;
using TMS.DomainObjects.Response;

namespace TMS.DomainGateway.Task.Interfaces
{
    public interface IOrderTask
    {
        OrderResponse CreateUpdateOrder(OrderRequest order);
        OrderSearchResponse GetOrders(OrderSearchRequest orderSearchRequest);
        PackingSheetResponse CreateUpdatePackingSheet(PackingSheetRequest packingSheetRequest);
        PackingSheetResponse GetPackingSheetDetails(int orderId);
        OrderTrackResponse TrackOrder(int orderId);
        CommonResponse GetOrderIds(string tokenValue);
        DealerDetailsResponse GetDealers(int orderId, string searchText);
        OrderDetailsResponse GetOrderDetails(int orderId);
        Partner GetPartnerDetail(string partnerNo, int uploadType);
        string GetBusinessAreaCode(int businessAreaId);
        OrderStatusResponse UpdateOrderStatus(OrderStatusRequest request);
        OrderStatusResponse CancelOrder(OrderStatusRequest request);
    }
}
