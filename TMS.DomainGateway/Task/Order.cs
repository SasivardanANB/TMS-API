﻿using System;
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
        public abstract HargaResponse GetHarga(HargaRequest request);
        public abstract ShipmentScheduleOcrResponse CreateOrderFromShipmentScheduleOcr(ShipmentScheduleOcrRequest request);
        public abstract OrderResponse OcrOrderResponse(ShipmentScheduleOcrRequest shipmentScheduleOcrRequest);
        public abstract OrderResponse CreateOrdersFromShipmentListOCR(OrderRequest request);
        public abstract InvoiceResponse GetInvoiceRequest(OrderStatusRequest request);
        public abstract OrderStatusResponse SwapeStopPoints(OrderStatusRequest orderStatusRequest);
        public abstract ImageGuidsResponse GetShippingListGuids(string orderNumber);
        public abstract ImageGuidsResponse GetPodGuids(string orderNumber);
        public abstract ImageGuidsResponse GetPhotoWithCustomerGuids(string orderNumber);
        public abstract OrderResponse GetShipmentSchedulesFromEmail(string token);
        public abstract void UpdateShipmentScheduleOCROrderStatus(string imageGUID, bool status, string message);
        public abstract PackingSheetResponse CreateUpdatePackingSheetDetailsDSM(ShipmentListRequest shipmentListRequest);

    }
}
