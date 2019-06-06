using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DMS.DataGateway.DataModels
{
    [Table("SubDistrict", Schema = "DMS")]
    public class SubDistrict : ModelBase
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [Required]
        [MaxLength(4)]
        [Index("SubDistrict_SubDistrictCode", IsUnique = true)]
        public string SubDistrictCode { get; set; }
        [MaxLength(50)]
        public string SubDistrictName { get; set; }
        [ForeignKey("City")]
        public int CityID { get; set; }
        public City City { get; set; }
       
    }
}
