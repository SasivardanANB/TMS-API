using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMS.DomainObjects.Objects
{
    public class DealerDetails
    {
        public int OrderDeatialId { get; set; }
        public int DealerId { get; set; }
        public string DealerNumber { get; set; }
        public string DealerName { get; set; }
    }
}
