using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DMS.DomainObjects.Objects;

namespace DMS.DataGateway.DataModels
{
    [Table("City", Schema = "DMS")]
    public class City : ModelBase
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [Required]
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
