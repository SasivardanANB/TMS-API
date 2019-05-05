using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMS.DomainObjects.Objects
{
    public class Partner
    {
        public int ID { get; set; }
        public string PartnerNo { get; set; }
        public string PartnerName { get; set; }
        public int PartnerTypeID { get; set; }
        public bool IsActive { get; set; }
    }
}
