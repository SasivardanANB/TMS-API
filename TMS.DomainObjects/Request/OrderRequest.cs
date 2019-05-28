using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMS.DomainObjects.Objects;

namespace TMS.DomainObjects.Request
{
    public class OrderRequest : RequestFilter
    {
        public List<Order> Requests { get; set; }
        [Range(1,2)]
        public int UploadType { get; set; }
    }
}
