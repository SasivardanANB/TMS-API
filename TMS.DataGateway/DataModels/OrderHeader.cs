using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMS.DataGateway.DataModels
{
    [Table("OrderHeader", Schema = "TMS")]
    public class OrderHeader
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [ForeignKey("BusinessArea")]
        public int BusinessAreaId { get; set; }
        public BusinessArea BusinessArea { get; set; }
        [MaxLength(50)]
        public string SOPONumber { get; set; }
        [Index("OrderHeader_OrderNo", IsUnique = true)]
        [Required]
        [MaxLength(15)]
        public string OrderNo { get; set; }
        public string LegecyOrderNo { get; set; }
        public DateTime OrderDate { get; set; }
        public int OrderType { get; set; }
        [ForeignKey("FleetType")]
        public int FleetTypeID { get; set; }
        public FleetType FleetType { get; set; }
        public string VehicleShipment { get; set; }
        public string DriverNo { get; set; }
        public string DriverName { get; set; }
        public string VehicleNo { get; set; }
        public decimal OrderWeight { get; set; }
        public string OrderWeightUM { get; set; }
        
        //[ForeignKey("OrderStatus")]
        public int OrderStatusID { get; set; }
        public bool IsActive { get; set; }
        public decimal Harga { get; set; }
        [ForeignKey("ImageGuid")]
        public int? ShipmentScheduleImageID { get; set; }
        public ImageGuid ImageGuid { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public string CreatedBy
        {
            get { return "SYSTEM"; }
            set { }
        }
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime CreatedTime
        {
            get { return DateTime.Now; }
            set { }
        }
        public string LastModifiedBy { get; set; }
        public DateTime? LastModifiedTime { get; set; }
    }
}
