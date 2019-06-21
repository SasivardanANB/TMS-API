using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMS.DomainObjects.Objects
{
    public class Order
    {
        [MaxLength(15)]
        public string LegecyOrderNo { get; set; }
        public int ID { get; set; }
        [Required, MaxLength(4)]
        public string BusinessArea { get; set; }
        [Required]
        [MaxLength(15)]
        public string OrderNo { get; set; }
        [Required]
        public int SequenceNo { get; set; }
        [Required]
        [MaxLength(10)]
        public string PartnerNo1 { get; set; }
        [Required]
        [Range(1, 4)]
        public int PartnerType1 { get; set; }
        [Required]
        [MaxLength(30)]
        public string PartnerName1 { get; set; }
        [Required]
        [MaxLength(10)]
        public string PartnerNo2 { get; set; }
        [Required]
        [Range(1, 4)]
        public int PartnerType2 { get; set; }
        [Required]
        [MaxLength(30)]
        public string PartnerName2 { get; set; }
        [Required]
        [MaxLength(10)]
        public string PartnerNo3 { get; set; }
        [Required]
        [Range(1, 4)]
        public int PartnerType3 { get; set; }
        [Required]
        [MaxLength(30)]
        public string PartnerName3 { get; set; }
        [Required]
        public int FleetType { get; set; }
        [Required]
        public int OrderType { get; set; }
        [MaxLength(50)]
        public string VehicleShipmentType { get; set; }
        [MaxLength(12)]
        public string DriverNo { get; set; }
        [MaxLength(30)]
        public string DriverName { get; set; }
        [MaxLength(12)]
        public string VehicleNo { get; set; }
        public decimal OrderWeight { get; set; }
        [MaxLength(5)]
        public string OrderWeightUM { get; set; }
        [Required]
        [RegularExpression(@"(3[01]|[12][0-9]|0?[1-9])\.(1[012]|0?[1-9])\.((?:19|20)\d{2})$", ErrorMessage = "Invalid date format.")]
        public string EstimationShipmentDate { get; set; }
        [Required]
        [RegularExpression(@"^(0[0-9]|1[0-9]|2[0-3]|[0-9])\:[0-5][0-9]$", ErrorMessage = "Invalid time format.")]
        public string EstimationShipmentTime { get; set; }
        [Required]
        [RegularExpression(@"(3[01]|[12][0-9]|0?[1-9])\.(1[012]|0?[1-9])\.((?:19|20)\d{2})$", ErrorMessage = "Invalid date format.")]
        public string ActualShipmentDate { get; set; }
        [Required]
        [RegularExpression(@"^(0[0-9]|1[0-9]|2[0-3]|[0-9])\:[0-5][0-9]$", ErrorMessage = "Invalid time format.")]
        public string ActualShipmentTime { get; set; }
        [MaxLength(50)]
        public string Sender { get; set; }
        [MaxLength(50)]
        public string Receiver { get; set; }
        [Required]
        public int OrderShipmentStatus { get; set; }
        [Required]
        [MaxLength(11)]
        public string Dimension { get; set; }
        [Required]
        public int TotalPallet { get; set; }
        [MaxLength(200)]
        public string Instructions { get; set; }
        [MaxLength(20)]
        public string ShippingListNo { get; set; }
        public string PackingSheetNo { get; set; }
        public int TotalCollie { get; set; }
        [MaxLength(20)]
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
        public decimal Harga { get; set; }
        public int? ShipmentScheduleImageID { get; set; }
        public string ShipmentScheduleImageGUID { get; set; }
        [MaxLength(10)]
        public string SOPONumber { get; set; }

    }
}
