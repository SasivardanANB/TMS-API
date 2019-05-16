using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMS.DataGateway.DataModels
{
    [Table("OrderDetail", Schema = "TMS")]
    public class OrderDetail
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [ForeignKey("OrderHeader")]
        public int OrderHeaderID { get; set; }
        public OrderHeader OrderHeader { get; set; }
        public int SequenceNo { get; set; }
        public string Sender { get; set; }
        public string Receiver { get; set; }
        public string Dimension { get; set; }
        public int TotalPallet { get; set; }
        public string Instruction { get; set; }
        public string ShippingListNo { get; set; }
        public int TotalCollie { get; set; }
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
        public string Katerangan { get; set; }
        public DateTime? LastModifiedTime { get; set; }
    }
}
