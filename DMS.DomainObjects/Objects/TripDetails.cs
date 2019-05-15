using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMS.DomainObjects.Objects
{
    public class TripDetails
    {
        public int ID { get; set; }
        public string TripNumber { get; set; }
        public string OrderNumber { get; set; }
        public string TransporterName { get; set; }
        public string TransporterCode { get; set; }
        public int UserId { get; set; }
        public string VehicleType { get; set; }
        public string VehicleNumber { get; set; }
        public string TripType { get; set; }
        public decimal Weight { get; set; }
        public string PoliceNumber { get; set; }
        public List<StopPoints> StopPoints { get; set; }
        public string TripStatus { get; set; }
        public int? TripStatusId { get; set; }
    }
}
