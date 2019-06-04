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
        [MaxLength(4)]
        [Index("PartnerType_PartnerTypeCode", IsUnique = true)]
        public string PartnerTypeCode { get; set; }
        [MaxLength(50)]
        public string PartnerTypeDescription { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public string CreatedBy
        {
            get { return "SYSTEM"; }
            set { }
        }
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime CreatedTime
        {
            get { return DateTime.Now; }
            set { }
        }
        public string LastModifiedBy { get; set; }
        public DateTime? LastModifiedTime { get; set; }
    }
}
