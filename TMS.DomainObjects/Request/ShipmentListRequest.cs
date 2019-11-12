using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMS.DomainObjects.Request
{
    public class ShipmentListRequest 
    {

        public List<ShipmentListDetails> Requests { get; set; }
        public string OrderNumber { get; set; }
        public string SequenceNumber { get; set; }
    }
}
