using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMS.DataGateway.DataModels
{
    [Table("RoleMenu", Schema = "TMS")]
    public class RoleMenu : ModelBase
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [ForeignKey("Role")]
        public int RoleID { get; set; }
        public Role Role { get; set; }
        [ForeignKey("Menu")]
        public int MenuID { get; set; }
        public Menu Menu { get; set; }
       
    }
}
