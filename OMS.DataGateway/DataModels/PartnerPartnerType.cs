using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMS.DataGateway.DataModels
{
    [Table("PartnerPartnerType", Schema = "OMS")]
    public class PartnerPartnerType :ModelBase
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [ForeignKey("Partner")]
        public int PartnerId { get; set; }
        [ForeignKey("PartnerType")]
        public int PartnerTypeId { get; set; }
        public PartnerType PartnerType { get; set; }
        public Partner Partner { get; set; }
    }
}
