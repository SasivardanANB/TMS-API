using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMS.DataGateway.Repositories.Interfaces;
using TMS.DomainGateway.Task;
using TMS.DomainObjects.Objects;
using TMS.DomainObjects.Request;
using TMS.DomainObjects.Response;

namespace TMS.BusinessGateway.Task
{
    public partial class BusinessOrderTask : OrderTask
    {
        private IOrder _orderRepository;
        public BusinessOrderTask(IOrder orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public override OrderResponse CreateUpdateOrder(OrderRequest order)
        {
            //If needed write business logic here for request.

            OrderResponse orderData = _orderRepository.CreateUpdateOrder(order);

            //If needed write business logic here for response.
            return orderData;
        }

        public override OrderSearchResponse GetOrders(OrderSearchRequest orderSearchRequest)
        {
            //If needed write business logic here for request.

            OrderSearchResponse orderSearchResponse = _orderRepository.GetOrders(orderSearchRequest);

            //If needed write business logic here for response.
            return orderSearchResponse;
        }
        public override PackingSheetResponse CreateUpdatePackingSheet(PackingSheetRequest packingSheetRequest)
        {
            PackingSheetResponse  packingSheetResponse = _orderRepository.CreateUpdatePackingSheet(packingSheetRequest);
            return packingSheetResponse;
        }

        public override PackingSheetResponse GetPackingSheetDetails(int orderId)
        {
            PackingSheetResponse packingSheetResponse = _orderRepository.GetPackingSheetDetails(orderId);
            return packingSheetResponse;
        }
        public override OrderTrackResponse TrackOrder(int orderId)
        {
            //If needed write business logic here for request.

            OrderTrackResponse orderTrackResponse = _orderRepository.TrackOrder(orderId);

            //If needed write business logic here for response.
            return orderTrackResponse;
        }

        public override CommonResponse GetOrderIds()
        {
            //If needed write business logic here for request.

            CommonResponse commonResponse  = _orderRepository.GetOrderIds();

            //If needed write business logic here for response.
            return commonResponse;
        }

        public override DealerDetailsResponse GetDealers(int orderId, string searchText)
        {
            DealerDetailsResponse dealerDetailsResponse = _orderRepository.GetDealers(orderId, searchText);
            return dealerDetailsResponse;
        }

        public override OrderDetailsResponse GetOrderDetails(int orderId)
        {
            OrderDetailsResponse orderDetailsResponse = _orderRepository.GetOrderDetails(orderId);
            return orderDetailsResponse;
        }
        public override Partner GetPartnerDetail(string partnerNo, int uploadType)
        {
            return _orderRepository.GetPartnerDetail(partnerNo, uploadType);
        }

        public override string GetBusinessAreaCode(int businessAreaId)
        {
            return _orderRepository.GetBusinessAreaCode(businessAreaId);
        }

        public override OrderStatusResponse UpdateOrderStatus(OrderStatusRequest request)
        {
            return _orderRepository.UpdateOrderStatus(request);
        }
    }

}
