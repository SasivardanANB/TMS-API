using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMS.DataGateway.DataModels
{
    [Table("PostalCode", Schema = "TMS")]
    public class PostalCode : ModelBase
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [Required()]
        [MaxLength(6)]
        [Index("PostalCode_PostalCodeNo", IsUnique = true)]
        public string PostalCodeNo { get; set; }
        [ForeignKey("SubDistrict")]
        public int SubDistrictID { get; set; }
        public SubDistrict SubDistrict { get; set; }
       
    }
}
