using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMS.DataGateway.DataModels
{
    [Table("UserApplication", Schema = "TMS")]
    public class UserApplication : ModelBase
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [ForeignKey("Application")]
        public int ApplicationID { get; set; }
        public Application Application { get; set; }
        [ForeignKey("User")]
        public int UserID { get; set; }
        public User User { get; set; }
      
    }
}
