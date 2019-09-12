using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DMS.DataGateway.DataModels
{
    [Table("Partner", Schema = "DMS")]
    public class Partner : ModelBase
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        [MaxLength(10)]
        [Index("Partner_PartnerNo", IsUnique = true)]
        public string PartnerNo { get; set; }
        [MaxLength(30)]
        public string PartnerName { get; set; }
        public bool IsActive { get; set; }
        
    }
}
