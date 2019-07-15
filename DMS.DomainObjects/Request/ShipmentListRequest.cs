using DMS.DomainObjects.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMS.DomainObjects.Request
{
    public class ShipmentListRequest : RequestFilter
    {
        public List<ShipmentListDetails> Requests { get; set; }
    }
}
