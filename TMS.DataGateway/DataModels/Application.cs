using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMS.DataGateway.DataModels
{
    [Table("Application", Schema = "TMS")]
    public class Application : ModelBase
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [Required(ErrorMessage = "")]
        [MaxLength(50)]
        [Index("Application_ApplicationCode", IsUnique = true)]
        public string ApplicationCode { get; set; }
        [MaxLength(100)]
        public string ApplicationName { get; set; }
        
    }
}
