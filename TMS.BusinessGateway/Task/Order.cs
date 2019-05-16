using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMS.DataGateway.Repositories.Interfaces;
using TMS.DomainGateway.Task;
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

        public override OrderTrackResponse TrackOrder(int orderId)
        {
            //If needed write business logic here for request.

            OrderTrackResponse orderTrackResponse = _orderRepository.TrackOrder(orderId);

            //If needed write business logic here for response.
            return orderTrackResponse;
        }
    }
}
