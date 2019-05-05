using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMS.DataGateway.DataModels
{
    [Table("OrderHeaderHSOAdditionalData", Schema = "TMS")]
    public class OrderHeaderHSOAdditionalData
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [Index("OrderHeaderHSOAdditionalData_OrderHeaderID", IsUnique = true)]
        public int OrderHeaderID { get; set; }
        public string ShipmentIDAHM { get; set; }
    }
}
