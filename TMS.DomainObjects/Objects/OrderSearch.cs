using System.ComponentModel.DataAnnotations;

namespace TMS.DomainObjects.Objects
{
    public class OrderSearch
    {
        public int OrderId { get; set; }
        [MaxLength(15)]
        public string OrderNumber { get; set; }
        public string Source { get; set; }
        public string Destination { get; set; }
        public string VehicleType { get; set; }
        public string ExpeditionName { get; set; }
        public string PoliceNumber { get; set; }
        public string OrderStatus { get; set; }
        [MaxLength(20)]
        public string PackingSheetNumber { get; set; }
        public int OrderType { get; set; }
        public bool IsOrderEditable { get; set; }
    }
}
