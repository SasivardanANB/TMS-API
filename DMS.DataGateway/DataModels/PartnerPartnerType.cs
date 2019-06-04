using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMS.DataGateway.DataModels
{
    [Table("PartnerPartnerType", Schema = "DMS")]
    public class PartnerPartnerType
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
        public string CreatedBy
        {
            get;
            set;
        }
        public DateTime? CreatedTime
        {
            get;
            set;
        }
        public string LastModifiedBy { get; set; }
        public DateTime? LastModifiedTime { get; set; }
    }
}
