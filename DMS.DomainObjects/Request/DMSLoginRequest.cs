using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMS.DomainObjects.Request
{
    public class DMSLoginRequest
    {
        [Required(ErrorMessageResourceType = typeof(Resource.DMSResource), ErrorMessageResourceName = "InvalidUserName")]
        
        public string UserName { get; set; }
        [Required(ErrorMessageResourceType = typeof(Resource.DMSResource), ErrorMessageResourceName = "InvalidUserPassword")]
        public string UserPassword { get; set; }
    }

}
