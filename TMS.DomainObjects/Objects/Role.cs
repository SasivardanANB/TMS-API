using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace TMS.DomainObjects.Objects
{
    public class Role
    {
        public int ID { get; set; }
        [MaxLength(4)]
        [Required(ErrorMessageResourceType = typeof(Resource.ResourceData), ErrorMessageResourceName = "InvalidRoleCode")]
        public string RoleCode { get; set; }
        [MaxLength(30)]
        public string RoleDescription { get; set; }
        public bool IsActive { get; set; }
        public List<RoleMenu> RoleMenus { get; set; }
    }
}
