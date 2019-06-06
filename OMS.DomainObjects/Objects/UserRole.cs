using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMS.DomainObjects.Objects
{
    public class UserRole
    {
        public int ID { get; set; }
        [Required(ErrorMessageResourceType = typeof(Resource.ResourceData), ErrorMessageResourceName = "InvalidUserID")]
        [Range(minimum: 1, maximum: int.MaxValue, ErrorMessageResourceType = typeof(Resource.ResourceData), ErrorMessageResourceName = "InvalidUserID")]
        public int UserID { get; set; }
        [MaxLength(30)]
        public string UserName { get; set; }
        [Required(ErrorMessageResourceType = typeof(Resource.ResourceData), ErrorMessageResourceName = "InvalidRole")]
        [Range(minimum: 1, maximum: int.MaxValue, ErrorMessageResourceType = typeof(Resource.ResourceData), ErrorMessageResourceName = "InvalidRole")]
        public int RoleID { get; set; }
        public string RoleName { get; set; }
        [Required(ErrorMessageResourceType = typeof(Resource.ResourceData), ErrorMessageResourceName = "InvalidRegion")]
        [Range(minimum: 1, maximum: int.MaxValue, ErrorMessageResourceType = typeof(Resource.ResourceData), ErrorMessageResourceName = "InvalidRegion")]
        public int BusinessAreaID { get; set; }
        public string BusinessArea { get; set; }

    }
}
