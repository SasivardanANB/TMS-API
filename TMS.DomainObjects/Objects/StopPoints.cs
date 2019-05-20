using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMS.DomainObjects.Objects
{
    public class StopPoints
    {
        public int ID { get; set; }
        public int PartnerId { get; set; }
        public string PartnerName { get; set; }
        public string PartnerCode { get; set; }
        public int PeartnerType { get; set; }
        public string SubDistrictName { get; set; }
        public string CityName { get; set; }
        public string ProvinceName { get; set; }
        public string Address { get; set; }
        public string EstimationShipmentDate { get; set; }
        public string ActualShipmentDate { get; set; }
        public int SequenceNo { get; set; }
    }
}
