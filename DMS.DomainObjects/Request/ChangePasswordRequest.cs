using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace DMS.DomainObjects.Request
{
  public  class ChangePasswordRequest
    {
        public int Id { set; get; }
        [Required]
        [MaxLength(30)]
        public string OldPassword { set; get; }
        [RegularExpression("^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{8,}$", ErrorMessageResourceType = typeof(Resource.ResourceData), ErrorMessageResourceName = "PasswordInvalid")]
        [Required(ErrorMessageResourceType = typeof(Resource.ResourceData), ErrorMessageResourceName = "InvalidUserPassword")]
        [MaxLength(30)]
        public string NewPassword { set; get; }
    }
}
