using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TMS.DataGateway.DataModels
{
    [Table("UserRoles", Schema = "TMS")]
    public class UserRoles : ModelBase
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [ForeignKey("User")]
        public int UserID { get; set; }
        public User User { get; set; }
        [ForeignKey("Role")]
        public int RoleID { get; set; }
        public Role Role { get; set; }
        [ForeignKey("BusinessArea")]
        public int BusinessAreaID { get; set; }
        public BusinessArea BusinessArea { get; set; }
        public bool IsDelete { get; set; }
        
    }
}
