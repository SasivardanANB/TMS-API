using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMS.DomainObjects.Objects
{
    public class Trip
    {
        public int ID { get; set; }
        public int OrderId { get; set; }
        public int OrderType { get; set; }
        public string VehicleType { get; set; }
        public string OrderNumber { get; set; }
        public string Source { get; set; }
        public string Destination { get; set; }
        public string Description { get; set; }
        public string Dimensions { get; set; }
        public string Vehicle { get; set; }
        public DateTime EstimatedShipmentDate { get; set; }
        public DateTime EstimatedArrivalDate { get; set; }
        public string OrderStatus { get; set; }
        public string PackingSheetNumber { get; set; }
        public string PoliceNumber { get; set; }
        public int OrderStatusId { get; set; }
        public bool IsChangeAllowed { get; set; }
    }
}
