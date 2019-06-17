using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMS.DomainObjects.Request
{
    public class LoginRequest
    {
        [Required(ErrorMessageResourceType = typeof(Resource.ResourceData), ErrorMessageResourceName = "InvalidUserName")]
        [MaxLength(30)]
        public string UserName { get; set; }
        [Required(ErrorMessageResourceType = typeof(Resource.ResourceData), ErrorMessageResourceName = "InvalidUserPassword")]
        [MaxLength(30)]
        public string UserPassword { get; set; }
        public string DeviceToken { get; set; }
    }
}
