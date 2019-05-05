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

        public override OrderResponse GetOrders(OrderRequest order)
        {
            //If needed write business logic here for request.

            OrderResponse orderData = _orderRepository.GetOrders(order);

            //If needed write business logic here for response.
            return orderData;
        }
    }
}
