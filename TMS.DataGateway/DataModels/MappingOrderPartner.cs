using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMS.DataGateway.DataModels
{
    [Table("MappingOrderPartner", Schema = "TMS")]
    public class MappingOrderPartner
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [Index("MappingOrderPartner_OrderTypeID", IsUnique = true)]
        public int OrderTypeID { get; set; }
        [Index("MappingOrderPartner_PartnerTypeID", IsUnique = true)]
        public int PartnerTypeID { get; set; }
    }
}
