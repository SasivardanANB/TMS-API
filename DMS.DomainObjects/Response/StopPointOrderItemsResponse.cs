using DMS.DomainObjects.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMS.DomainObjects.Response
{
    public class StopPointOrderItemsResponse : Message
    {
        public List<ShipmentListDetails> Data { get; set; }
    }
}
