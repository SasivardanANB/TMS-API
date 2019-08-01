using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMS.DomainObjects.Objects;

namespace TMS.DomainObjects.Request
{
    public class OrderStatusRequest : RequestFilter
    {
        public List<OrderStatus> Requests { get; set; }
        public string RequestFrom { get; set; }
    }
}
