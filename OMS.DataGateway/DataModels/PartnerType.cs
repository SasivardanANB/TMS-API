using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMS.DataGateway.DataModels
{
    [Table("PartnerType", Schema = "OMS")]
    public class PartnerType : ModelBase
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [Required(ErrorMessage = "")]
        [MaxLength(4)]
        [Index("PartnerType_PartnerTypeCode", IsUnique = true)]
        public string PartnerTypeCode { get; set; }
        [MaxLength(50)]
        public string PartnerTypeDescription { get; set; }
        
    }
}
