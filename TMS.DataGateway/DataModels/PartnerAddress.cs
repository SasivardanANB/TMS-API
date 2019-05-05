using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMS.DataGateway.DataModels
{
    [Table("PartnerAddress", Schema = "TMS")]
    public class PartnerAddress
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [Index("PartnerAddress_PartnerID", IsUnique = true)]
        public int PartnerID { get; set; }
        [MaxLength(200)]
        [Index("PartnerAddress_Address", IsUnique = true)]
        public string Address { get; set; }
        public string Phone { get; set; }
        public int PostalCodeID { get; set; }
        public int IsDefault { get; set; }
    }
}
