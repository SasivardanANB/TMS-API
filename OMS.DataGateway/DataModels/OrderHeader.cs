using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMS.DataGateway.DataModels
{
    [Table("OrderHeader", Schema = "OMS")]
    public class OrderHeader
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [ForeignKey("BusinessArea")]
        public int BusinessAreaId { get; set; }
        public BusinessArea BusinessArea { get; set; }
        [Index("OrderHeader_OrderNo", IsUnique = true)]
        [Required]
        [MaxLength(15)]
        public string OrderNo { get; set; }
        public string LegecyOrderNo { get; set; }
        public DateTime OrderDate { get; set; }
        public int OrderType { get; set; }
        public int FleetType { get; set; }
        public string VehicleShipment { get; set; }
        public string DriverNo { get; set; }
        public string DriverName { get; set; }
        public string VehicleNo { get; set; }
        public decimal OrderWeight { get; set; }
        public string OrderWeightUM { get; set; }
        
        //[ForeignKey("OrderStatus")]
        public int OrderStatusID { get; set; }
        //public OrderStatus OrderStatus { get; set; }
        public bool IsActive { get; set; }
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
