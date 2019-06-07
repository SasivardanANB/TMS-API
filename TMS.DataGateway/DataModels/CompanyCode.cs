using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMS.DataGateway.DataModels
{
    [Table("CompanyCode", Schema = "TMS")]
    public class CompanyCode : ModelBase
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [Required()]
        [MaxLength(4)]
        [Index("CompanyCode_CompanyCodeCode", IsUnique = true)]
        public string CompanyCodeCode { get; set; }
        [MaxLength(200)]
        public string CompanyCodeDescription { get; set; }
       
    }
}
