using OMS.DataGateway.Repositories;
using OMS.DataGateway.Repositories.Iterfaces;
using OMS.DomainGateway.Task;
using OMS.DomainObjects.Request;
using OMS.DomainObjects.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMS.BusinessGateway.Task
{
    public partial class BusinessOrderTask : OrderTask
    {
        private IOrder _orderRepository;
        public BusinessOrderTask(IOrder orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public override OrderResponse GetOrders(DownloadOrderRequest order)
        {
            //If needed write business logic here for request.

            OrderResponse orderData = _orderRepository.GetOrders(order);

            //If needed write business logic here for response.
            return orderData;
        }

        public override OrderResponse CreateUpdateOrders(OrderRequest request)
        {
            OrderResponse orderData = _orderRepository.CreateUpdateOrders(request);
            return orderData;
        }
        public override OrderStatusResponse GetAllOrderStatus()
        {
            OrderStatusResponse orderStatusData = _orderRepository.GetAllOrderStatus();
            return orderStatusData;
        }
    }
}
