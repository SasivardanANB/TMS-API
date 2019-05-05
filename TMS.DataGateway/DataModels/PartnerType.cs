using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMS.DataGateway.DataModels
{
    [Table("PartnerType", Schema = "TMS")]
    public class PartnerType
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [Required(ErrorMessage = "")]
        [MaxLength(10)]
        [Index("PartnerType_PartnerTypeCode", IsUnique = true)]
        public string PartnerTypeCode { get; set; }
        public string PartnerTypeDescription { get; set; }
    }
}
