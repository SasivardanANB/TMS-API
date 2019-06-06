using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DMS.DataGateway.DataModels
{
    [Table("PostalCode", Schema = "DMS")]
    public class PostalCode : ModelBase
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [Required(ErrorMessage = "")]
        [MaxLength(6)]
        [Index("PostalCode_PostalCodeNo", IsUnique = true)]
        public string PostalCodeNo { get; set; }
        [ForeignKey("SubDistrict")]
        public int SubDistrictID { get; set; }
        public SubDistrict SubDistrict { get; set; }
        
    }
}
