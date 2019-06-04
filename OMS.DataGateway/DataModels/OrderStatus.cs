using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMS.DataGateway.DataModels
{
    [Table("OrderStatus", Schema = "OMS")]
    public class OrderStatus : ModelBase
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [MaxLength(4)]
        public string OrderStatusCode { get; set; }
        [MaxLength(50)]
        public string OrderStatusValue { get; set; }
       

    }
}
