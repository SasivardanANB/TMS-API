using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMS.DomainObjects.Objects
{
    public class PartnerSearch
    {
        public int UserId { get; set; }
        public int PartnerTypeId { get; set; }
        public string SearchText { get; set; }
    }
}
