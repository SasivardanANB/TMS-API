using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMS.DomainObjects.Request;
using TMS.DomainObjects.Response;

namespace TMS.DataGateway.Repositories.Interfaces
{
    public interface IOrder
    {
        OrderResponse CreateUpdateOrder(OrderRequest request);
        OrderSearchResponse GetOrders(OrderSearchRequest orderSearchRequest);
        PackingSheetResponse CreateUpdatePackingSheet(PackingSheetRequest packingSheetRequest);
        OrderTrackResponse TrackOrder(int orderId);
        CommonResponse GetOrderIds();
        DealerDetailsResponse GetDealers(int orderId, string searchText);
        OrderDetailsResponse GetOrderDetails(int orderId);
        DomainObjects.Objects.Partner GetPartnerDetail(string partnerNo);
        string GetBusinessAreaCode(int businessAreaId);
    }
}
