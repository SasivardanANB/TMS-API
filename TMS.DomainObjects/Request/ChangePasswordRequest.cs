using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMS.DomainObjects.Request
{
    public class ChangePasswordRequest
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(30)]
        public string OldPassword { get; set; }
        [RegularExpression("^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{8,}$", ErrorMessageResourceType = typeof(Resource.ResourceData), ErrorMessageResourceName = "PasswordInvalid")]
        [Required(ErrorMessageResourceType = typeof(Resource.ResourceData), ErrorMessageResourceName = "InvalidUserPassword")]
        [MaxLength(30)]
        public string NewPassword { get; set; }
    }
}
