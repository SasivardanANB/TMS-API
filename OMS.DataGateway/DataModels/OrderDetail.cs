using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMS.DataGateway.DataModels
{
    [Table("OrderDetail", Schema = "OMS")]
    public class OrderDetail : ModelBase
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [ForeignKey("OrderHeader")]
        public int OrderHeaderID { get; set; }
        public OrderHeader OrderHeader { get; set; }
        public int SequenceNo { get; set; }
        [MaxLength(50)]
        public string Sender { get; set; }
        [MaxLength(50)]
        public string Receiver { get; set; }
        [MaxLength(11)]
        public string Dimension { get; set; }
        public int TotalPallet { get; set; }
        [MaxLength(200)]
        public string Instruction { get; set; }
        [MaxLength(20)]
        public string ShippingListNo { get; set; }
        public int TotalCollie { get; set; }
        public DateTime EstimationShipmentDate { get; set; }
        public DateTime ActualShipmentDate { get; set; }
        public string Katerangan { get; set; }
    }
}
