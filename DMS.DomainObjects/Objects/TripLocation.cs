using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMS.DomainObjects.Objects
{
    public class TripLocation
    {
        public int ID { get; set; }
        public string PartnerNo { get; set; }
        public string PartnerName { get; set; }
        public int SequnceNumber { get; set; }
        public DateTime ActualDeliveryDate { get; set; }
        public DateTime EstimatedDeliveryDate { get; set; }
        public int PartnerType { get; set; }
    }
}
