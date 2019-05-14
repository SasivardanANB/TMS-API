using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMS.DomainObjects.Objects;

namespace TMS.DomainObjects.Request
{
    public class OrderRequest : RequestFilter
    {
        public List<Order> Requests { get; set; }
        public int UploadType { get; set; }
    }
}
