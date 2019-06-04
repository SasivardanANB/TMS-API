using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMS.DataGateway.DataModels
{
    [Table("Role", Schema = "OMS")]
    public class Role : ModelBase
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [Required(ErrorMessage = "")]
        [MaxLength(4)]
        public string RoleCode { get; set; }
        [MaxLength(30)]
        public string RoleDescription { get; set; }
        public bool IsActive { get; set; }
        public bool IsDelete { get; set; }
        
    }
}
