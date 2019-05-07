using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMS.DomainObjects.Objects
{
    public class Order
    {
        public string LegecyOrderNo { get; set; }
        public int ID { get; set; }
        [Required, MaxLength(5)]
        public string BusinessArea { get; set; }
        [Required]
        public string OrderNo { get; set; }
        [Required]
        public int SequenceNo { get; set; }
        [Required]
        public string PartnerNo1 { get; set; }
        [Required]
        [Range(1, 4)]
        public int PartnerType1 { get; set; }
        [Required]
        public string PartnerName1 { get; set; }
        [Required]
        public string PartnerNo2 { get; set; }
        [Required]
        [Range(1, 4)]
        public int PartnerType2 { get; set; }
        [Required]
        public string PartnerName2 { get; set; }
        [Required]
        public string PartnerNo3 { get; set; }
        [Required]
        [Range(1, 4)]
        public int PartnerType3 { get; set; }
        [Required]
        public string PartnerName3 { get; set; }
        [Required]
        public int FleetType { get; set; }
        [Required]
        public int OrderType { get; set; }
        public string VehicleShipmentType { get; set; }
        public string DriverNo { get; set; }
        public string DriverName { get; set; }
        public string VehicleNo { get; set; }
        public decimal OrderWeight { get; set; }
        public string OrderWeightUM { get; set; }
        [Required]
        [RegularExpression(@"(((0|1)[0-9]|2[0-9]|3[0-1])\/(0[1-9]|1[0-2])\/((19|20)\d\d))$", ErrorMessage = "Invalid date format.")]
        public string EstimationShipmentDate { get; set; }
        [Required]
        [RegularExpression(@"^(0[0-9]|1[0-9]|2[0-3]|[0-9]):[0-5][0-9]$", ErrorMessage = "Invalid time format.")]
        public string EstimationShipmentTime { get; set; }
        [Required]
        [RegularExpression(@"(((0|1)[0-9]|2[0-9]|3[0-1])\/(0[1-9]|1[0-2])\/((19|20)\d\d))$", ErrorMessage = "Invalid date format.")]
        public string ActualShipmentDate { get; set; }
        [Required]
        [RegularExpression(@"^(0[0-9]|1[0-9]|2[0-3]|[0-9]):[0-5][0-9]$", ErrorMessage = "Invalid time format.")]
        public string ActualShipmentTime { get; set; }
        public string Sender { get; set; }
        public string Receiver { get; set; }
        [Required]
        public int OrderShipmentStatus { get; set; }
        [Required]
        public string Dimension { get; set; }
        [Required]
        public int TotalPallet { get; set; }
        public string Instructions { get; set; }
        public string ShippingListNo { get; set; }
        public string PackingSheetNo { get; set; }
        public int TotalCollie { get; set; }
        public string ShipmentSAPNo { get; set; }
        public int BusinessAreaId { get; set; }
        public bool IsActive { get; set; }
        public DateTime OrderDate { get; set; }

        public DateTime ActualShipment { get; set; }
        public DateTime EstimationShipment { get; set; }
        public string OrderCreatedBy { get; set; }
        public DateTime OrderCreatedTime { get; set; }
        public string OrderLastModifiedBy { get; set; }
        public DateTime? OrderLastModifiedTime { get; set; }
        public int OrderDetailID { get; set; }
    }
}
