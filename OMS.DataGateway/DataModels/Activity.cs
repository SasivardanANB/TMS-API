using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMS.DataGateway.DataModels
{
    [Table("Activity", Schema = "OMS")]
    public class Activity : ModelBase
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [Required()]
        [MaxLength(50)]
        [Index("Activity_ActivityCode", IsUnique = true)]
        public string ActivityCode { get; set; }
        [MaxLength(225)]
        public string ActivityDescription { get; set; }
    }
}
