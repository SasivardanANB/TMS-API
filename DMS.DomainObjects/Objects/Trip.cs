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
        [MaxLength(15)]
        public string OrderNumber { get; set; }
        [MaxLength(50)]
        public string TripNumber { get; set; }
        [MaxLength(50)]
        public string TransporterName { get; set; }
        [MaxLength(10)]
        public string TransporterCode { get; set; }
        [MaxLength(12)]
        public string DriverNo { get; set; }
        [MaxLength(30)]
        public string DriverName { get; set; }
        public string VehicleType { get; set; }
        [MaxLength(12)]
        public string VehicleNumber { get; set; }
        [MaxLength(10)]
        public string TripType { get; set; }
        public decimal Weight { get; set; }
        [MaxLength(12)]
        //public string PoliceNumber { get; set; }
        public string TripStatusCode { get; set; }
        public int OrderType { get; set; }
        [MaxLength(4)]
        public string BusinessAreaCode { get; set; }
        public List<TripLocation> TripLocations { get; set; }
        public string ShipmentScheduleImageGUID { get; set; }
    }
}
