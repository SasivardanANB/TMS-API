using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMS.DomainObjects.Objects
{
    public class OrderSearch
    {
        public int OrderId { get; set; }
        public string OrderNumber { get; set; }
        public string Source { get; set; }
        public string Destination { get; set; }
        public string VehicleType { get; set; }
        public string ExpeditionName { get; set; }
        public string PoliceNumber { get; set; }
        public string OrderStatus { get; set; }
        public string PackingSheetNumber { get; set; }
        public int OrderType { get; set; }
    }
}
