using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMS.DomainGateway.Task.Interfaces;
using TMS.DomainObjects.Objects;
using TMS.DomainObjects.Request;
using TMS.DomainObjects.Response;

namespace TMS.DomainGateway.Task
{
    public abstract class OrderTask : IOrderTask
    {
        public abstract OrderResponse CreateUpdateOrder(OrderRequest order);
        public abstract OrderSearchResponse GetOrders(OrderSearchRequest orderSearchRequest);
        public abstract PackingSheetResponse CreateUpdatePackingSheet(PackingSheetRequest packingSheetRequest);
        public abstract PackingSheetResponse GetPackingSheetDetails(int orderId);
        public abstract OrderTrackResponse TrackOrder(int orderId);
        public abstract CommonResponse GetOrderIds(string tokenValue);
        public abstract DealerDetailsResponse GetDealers(int orderId, string searchText);

        public abstract OrderDetailsResponse GetOrderDetails(int orderId);
        public abstract Partner GetPartnerDetail(string partnerNo, int uploadType);
        public abstract string GetBusinessAreaCode(int businessAreaId);
        public abstract OrderStatusResponse UpdateOrderStatus(OrderStatusRequest request);
        public abstract OrderStatusResponse CancelOrder(OrderStatusRequest request);
    }
}
