using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMS.DataGateway.DataModels
{
    [Table("PartnerPIC", Schema = "TMS")]
    public class PartnerPIC
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [Index("PartnerPIC_PartnerID", IsUnique = true)]
        public int PartnerID { get; set; }
        [Index("PartnerPIC_PICID", IsUnique = true)]
        public int PICID { get; set; }
    }
}
