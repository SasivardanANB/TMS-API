using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMS.DataGateway.DataModels
{
    [Table("SubDistrict", Schema = "TMS")]
    public class SubDistrict : ModelBase
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [Required()]
        [MaxLength(4)]
        [Index("SubDistrict_SubdistrictCode", IsUnique = true)]
        public string SubdistrictCode { get; set; }
        [MaxLength(50)]
        public string SubdistrictName { get; set; }
        [ForeignKey("City")]
        public int CityID { get; set; }
        public City City { get; set; }
        
    }
}
