using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace OMS.DomainObjects.Objects
{
    public class Trip
    {
        public int ID { get; set; }
        public int OrderId { get; set; }
        public int OrderType { get; set; }
        public string VehicleType { get; set; }
        [MaxLength(15)]
        public string OrderNumber { get; set; }
        public string Source { get; set; }
        public string Destination { get; set; }
        [MaxLength(50)]
        public string Description { get; set; }
        [MaxLength(11)]
        public string Dimensions { get; set; }
        public string Vehicle { get; set; }
        public DateTime EstimatedShipmentDate { get; set; }
        public DateTime EstimatedArrivalDate { get; set; }
        public string OrderStatus { get; set; }
        [MaxLength(20)]
        public string PackingSheetNumber { get; set; }
        public string PoliceNumber { get; set; }
        public int OrderStatusId { get; set; }
        public bool IsChangeAllowed { get; set; }
        [MaxLength(12)]
        public string DriverNo { get; set; }
        [MaxLength(30)]
        public string DriverName { get; set; }
    }
}
