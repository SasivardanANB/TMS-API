using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMS.DomainObjects.Objects
{
    public class TripLocation
    {
        public int ID { get; set; }
        public string TypeofLocation { get; set; }
        public string Place { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string CityCode { get; set; }
        public string  ProvinceCode { get; set; }
        public int SequnceNumber { get; set; }
        public DateTime ActualDeliveryDate { get; set; }
        public DateTime EstimatedDeliveryDate { get; set; }
    }
}
