using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMS.DataGateway.DataModels
{
    [Table("OrderPointType", Schema = "TMS")]
    public class OrderPointType
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [Required(ErrorMessage = "")]
        [MaxLength(10)]
        [Index("OrderPointType_PartnerRoleCode", IsUnique = true)]
        public string PartnerRoleCode { get; set; }
        public string PartnerRoleDescription { get; set; }
    }
}
