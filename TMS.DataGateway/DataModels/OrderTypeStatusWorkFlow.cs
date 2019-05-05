using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMS.DataGateway.DataModels
{
    [Table("OrderTypeStatusWorkFlow", Schema = "TMS")]
    public class OrderTypeStatusWorkFlow
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [Index("OrderTypeStatusWorkFlow_TipeOrderID", IsUnique = true)]
        public int TipeOrderID { get; set; }
        [Index("OrderTypeStatusWorkFlow_OrderStatus", IsUnique = true)]
        public int OrderStatus { get; set; }
        [Index("OrderTypeStatusWorkFlow_StepNo", IsUnique = true)]
        public int StepNo { get; set; }
        public int IsOptional { get; set; }
        public int isTrackable { get; set; }
    }
}
