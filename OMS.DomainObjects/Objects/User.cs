using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace OMS.DomainObjects.Objects
{
    public class User
    {
        public int ID { get; set; }
        [Required(ErrorMessageResourceType = typeof(Resource.ResourceData), ErrorMessageResourceName = "InvalidUserName")]
        [MaxLength(30)]
        public string UserName { get; set; }
        [Required(ErrorMessageResourceType = typeof(Resource.ResourceData), ErrorMessageResourceName = "InvalidUserPassword")]
        [MaxLength(30)]
        [RegularExpression("^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{8,}$", ErrorMessageResourceType = typeof(Resource.ResourceData), ErrorMessageResourceName = "PasswordInvalid")]
        public string Password { get; set; }
        [Required(ErrorMessageResourceType = typeof(Resource.ResourceData), ErrorMessageResourceName = "InvalidConfirmUserPassword")]
        [Compare("Password",ErrorMessageResourceType = typeof(Resource.ResourceData), ErrorMessageResourceName = "PasswordMismatch")]
        public string ConfirmPassword { get; set; }
        [Required(ErrorMessageResourceType = typeof(Resource.ResourceData), ErrorMessageResourceName = "InvalidFirstName")]
        [MaxLength(30)]
        public string FirstName { get; set; }
        [Required(ErrorMessageResourceType = typeof(Resource.ResourceData), ErrorMessageResourceName = "InvalidLastName")]
        [MaxLength(30)]
        public string LastName { get; set; }
        [Required(ErrorMessageResourceType =typeof(Resource.ResourceData),ErrorMessageResourceName ="InvalidEmail")]
        public string Email { get; set; }
        public List<int> Applications { get; set; }
        public bool IsActive { get; set; }
        public List<string> ApplicationNames { get; set; }
        public List<Role> Roles { get; set; }
        public List<Region> Regions { get; set; }
    }
}
