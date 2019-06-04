using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMS.DataGateway.DataModels
{
    [Table("City", Schema = "OMS")]
    public class City : ModelBase
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [Required()]
        [MaxLength(4)]
        [Index("City_CityCode", IsUnique = true)]
        public string CityCode { get; set; }
        [MaxLength(50)]
        public string CityDescription { get; set; }
        [ForeignKey("Province")]
        public int ProvinceID { get; set; }
        public Province Province { get; set; }
       
    }
}
