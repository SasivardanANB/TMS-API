using OMS.DomainObjects.Request;
using OMS.DomainObjects.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMS.DomainGateway.Task.Interfaces
{
    public interface IOrderTask
    {
        OrderResponse GetOrders(DownloadOrderRequest order);
        OrderResponse CreateUpdateOrders(OrderRequest request);
    }
}
