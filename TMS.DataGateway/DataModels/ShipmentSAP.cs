using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMS.DataGateway.DataModels
{
    [Table("ShipmentSAP", Schema = "TMS")]
    public class ShipmentSAP : ModelBase
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [ForeignKey("OrderDetail")]
        public int OrderDetailID { get; set; }
        public OrderDetail OrderDetail { get; set; }
        [MaxLength(20)]
        public string ShipmentSAPNo { get; set; }
       
    }
}
