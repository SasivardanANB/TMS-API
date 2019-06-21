using OMS.DomainGateway.Task.Interfaces;
using OMS.DomainObjects.Request;
using OMS.DomainObjects.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMS.DomainGateway.Task
{
    public abstract class OrderTask : IOrderTask
    {
        public abstract OrderResponse GetOrders(DownloadOrderRequest order);
        public abstract OrderResponse CreateUpdateOrders(OrderRequest request);
        public abstract OrderStatusCodesResponse GetAllOrderStatus();
        public abstract OrderResponse SyncOrders(OrderRequest request);
        public abstract PackingSheetResponse CreateUpdatePackingSheet(PackingSheetRequest packingSheetRequest);
        public abstract OrderStatusResponse UpdateOrderStatus(OrderStatusRequest request);
    }
}
