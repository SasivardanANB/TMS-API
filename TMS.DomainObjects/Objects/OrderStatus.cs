using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMS.DomainObjects.Objects
{
    public class OrderStatus
    {
        public int OrderType { get; set; }
        public string OrderNumber { get; set; }
        public int SequenceNumber { get; set; }
        public int? IsLoad { get; set; }
        public string Remarks { get; set; }
        public string OrderStatusCode { get; set; }
    }
}
