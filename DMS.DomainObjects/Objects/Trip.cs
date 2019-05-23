using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMS.DomainObjects.Objects
{
    public class Trip
    {
        public int ID { get; set; }
        public string OrderNumber { get; set; }
        public string TripNumber { get; set; }
        public string TransporterName { get; set; }
        public string TransporterCode { get; set; }
        public string DriverName { get; set; }
        public string VehicleType { get; set; }
        public string VehicleNumber { get; set; }
        public string TripType { get; set; }
        public decimal Weight { get; set; }
        public string PoliceNumber { get; set; }
        public string TripStatusCode { get; set; }
        public int OrderType { get; set; }
        public string BusinessAreaCode { get; set; }
        public List<TripLocation> TripLocations { get; set; }
    }
}
