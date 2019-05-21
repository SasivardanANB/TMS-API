using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMS.DataGateway.DataModels
{
    [Table("OrderStatusHistory", Schema = "TMS")]
    public class OrderStatusHistory
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [ForeignKey("OrderDetail")]
        public int OrderDetailID { get; set; }
        public OrderDetail OrderDetail { get; set; }
        [ForeignKey("OrderStatus")]
        public int OrderStatusID { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public DateTime StatusDate { get; set; }
        public string Remarks { get; set; }
        public bool? IsLoad { get; set; }
    }
}
