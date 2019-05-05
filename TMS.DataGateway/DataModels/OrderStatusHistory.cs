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
        [Index("OrderStatusHistory_OrderHeaderID", IsUnique = true)]
        public int OrderHeaderID { get; set; }
        [Index("OrderStatusHistory_OrderStatus", IsUnique = true)]
        public int OrderStatus { get; set; }
        [Index("OrderStatusHistory_StepNo", IsUnique = true)]
        public int StepNo { get; set; }
        public int IsOptional { get; set; }
        public DateTime CreatedTime { get; set; }
        public DateTime CreatedBy { get; set; }
        public DateTime LastModififiedTime { get; set; }
        public DateTime LastModifiedBy { get; set; }
    }
}
