using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMS.DataGateway.DataModels
{
    [Table("PartnerTypeFunction", Schema = "TMS")]
    public class PartnerTypeFunction
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [Index("PartnerTypeFunction_PartnerID", IsUnique = true)]
        public int PartnerID { get; set; }
        [Index("PartnerTypeFunction_PartnerTypeID", IsUnique = true)]
        public int PartnerTypeID { get; set; }
    }
}
