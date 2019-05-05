using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DMS.DataGateway.DataModels
{
    [Table("Location", Schema = "DMS")]
    public class Location
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public string TypeofLocation { get; set; }
        [ForeignKey("City")]
        public int CityId { get; set; }
        [Required(ErrorMessage = "")]
        [MaxLength(50)]
        [Index("Location_Name", IsUnique = true)]
        public string Name { get; set; }
        public string Place { get; set; }
        public string Address { get; set; }

        public City City { get; set; }      
    }
}
