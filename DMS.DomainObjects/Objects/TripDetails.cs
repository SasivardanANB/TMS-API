using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMS.DomainObjects.Objects
{
    public class TripDetails
    {
        public int ID { get; set; }
        [MaxLength(50)]
        public string TripNumber { get; set; }
        [MaxLength(15)]
        public string OrderNumber { get; set; }
        [MaxLength(50)]
        public string TransporterName { get; set; }
        [MaxLength(10)]
        public string TransporterCode { get; set; }
        public int UserId { get; set; }
        public string VehicleType { get; set; }
        [MaxLength(12)]
        public string VehicleNumber { get; set; }
        [MaxLength(10)]
        public string TripType { get; set; }
        public decimal Weight { get; set; }
        [MaxLength(12)]
        public string PoliceNumber { get; set; }
        public List<StopPoints> StopPoints { get; set; }
        [MaxLength(30)]
        public string TripStatus { get; set; }
        public int? TripStatusId { get; set; }
        [Range(0, 2)]
        public int OrderType { get; set; }
        public DateTime TripDate { get; set; }
        public DateTime? LastModifiedTime { get; set; }
        public string ShipmentScheduleImageGUID { get; set; }
    }
}
