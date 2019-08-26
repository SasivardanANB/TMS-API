using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMS.DomainObjects.Request
{
    public class LoginRequest
    {
        [MaxLength(30)]
        [Required(ErrorMessageResourceType = typeof(Resource.ResourceData), ErrorMessageResourceName = "InvalidUserName")]
        public string UserName { get; set; }
        [MaxLength(30)]
        [Required(ErrorMessageResourceType = typeof(Resource.ResourceData), ErrorMessageResourceName = "InvalidUserPassword")]
        public string UserPassword { get; set; }
        public bool IsSAMALogin { get; set; }
        public string FirebaseToken { get; set; }
    }
}
