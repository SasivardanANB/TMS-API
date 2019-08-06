using System.ComponentModel.DataAnnotations;

namespace TMS.DomainObjects.Objects
{
    public class Gate
    {
        public int ID { get; set; }
        public int OrderId { get; set; }
        public string GateTypeDescription { get; set; }
        public int GateTypeId { get; set; }
        [MaxLength(12)]
        public string VehicleNumber { get; set; }
        public string OrderType { get; set; }
        public string VehicleTypeName { get; set; }
        public string Status { get; set; }
        public int GateId { get; set; }
        public string GateName { get; set; }
        [MaxLength(120)]
        public string Info { get; set; }
        public int BusinessAreaId { get; set; }
        public string BusinessArea { get; set; }
    }
}
