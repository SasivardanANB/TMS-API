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
        PackingSheetResponse GetPackingSheetDetails(int orderId);
        OrderTrackResponse TrackOrder(int orderId);
        CommonResponse GetOrderIds(string tokenValue);
        DealerDetailsResponse GetDealers(int orderId, string searchText);
        OrderDetailsResponse GetOrderDetails(int orderId);
        DomainObjects.Objects.Partner GetPartnerDetail(string partnerNo, int uploadType);
        string GetBusinessAreaCode(int businessAreaId);
        OrderStatusResponse UpdateOrderStatus(OrderStatusRequest request);
        OrderStatusResponse CancelOrder(OrderStatusRequest request);
        HargaResponse GetHarga(HargaRequest request);
        ShipmentScheduleOcrResponse CreateOrderFromShipmentScheduleOcr(ShipmentScheduleOcrRequest request);
        OrderResponse OcrOrderResponse(ShipmentScheduleOcrRequest shipmentScheduleOcrRequest);
        OrderResponse CreateOrdersFromShipmentListOCR(OrderRequest request);
        InvoiceResponse GetInvoiceRequest(OrderStatusRequest request);
        OrderStatusResponse SwapeStopPoints(OrderStatusRequest orderStatusRequest);
    }
}
