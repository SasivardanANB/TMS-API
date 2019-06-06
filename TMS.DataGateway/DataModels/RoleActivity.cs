using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMS.DataGateway.DataModels
{
    [Table("RoleActivity", Schema = "TMS")]
    public class RoleActivity 
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public int RoleID { get; set; }
        [MaxLength(4)]
        public string ActivityId { get; set; }
       
    }
}
