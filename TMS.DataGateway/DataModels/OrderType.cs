using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMS.DataGateway.DataModels
{
    [Table("OrderType",Schema ="TMS")]
    public class OrderType
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [Required(ErrorMessage ="")]
        [MaxLength(4)]
        [Index("OrderType_OrderTypeCode", IsUnique = true)]
        public string OrderTypeCode { get; set; }
        [MaxLength(30)]
        public string OrderTypeDescription { get; set; }
    }
}
