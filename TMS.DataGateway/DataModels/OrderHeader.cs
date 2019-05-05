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
        [MaxLength(4)]
        public string CompanyCodeID { get; set; }
        public string BusinessAreaID { get; set; }
        [MaxLength(10)]
        [Index("OrderHeader_TipeOrder", IsUnique = true)]
        public string TipeOrder { get; set; }
        [MaxLength(10)]
        [Index("OrderHeader_OrderNo", IsUnique = true)]
        public string OrderNo { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime EstimatedPickupTime { get; set; }
        public DateTime ActualPickupTime { get; set; }
        public DateTime EstimatedArrivalTime { get; set; }
        public DateTime ActualArrivalTime { get; set; }
        public decimal TotalPrice { get; set; }
        public int OrderStatusID { get; set; }
        public string Sender { get; set; }
        public string Receiver { get; set; }
        public string Instruction { get; set; }
        public string VehicleType { get; set; }
        public string PoliceNo { get; set; }
        public int TotalOfWeight { get; set; }
    }
}
