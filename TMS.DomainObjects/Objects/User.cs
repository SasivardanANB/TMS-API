using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace TMS.DomainObjects.Objects
{
    public class User
    {
        public int ID { get; set; }
        [MaxLength(30)]
        [Required(ErrorMessageResourceType = typeof(Resource.ResourceData), ErrorMessageResourceName = "InvalidUserName")]
        public string UserName { get; set; }
        [Required(ErrorMessageResourceType = typeof(Resource.ResourceData), ErrorMessageResourceName = "InvalidUserPassword")]
        [MaxLength(30)]
        public string Password { get; set; }
        [MaxLength(30)]
        [Required(ErrorMessageResourceType = typeof(Resource.ResourceData), ErrorMessageResourceName = "InvalidConfirmUserPassword")]
        [Compare("Password", ErrorMessageResourceType = typeof(Resource.ResourceData), ErrorMessageResourceName = "PasswordMismatch")]
        public string ConfirmPassword { get; set; }
        [MaxLength(30)]
        [Required(ErrorMessageResourceType = typeof(Resource.ResourceData), ErrorMessageResourceName = "InvalidFirstName")]
        public string FirstName { get; set; }
        [Required(ErrorMessageResourceType = typeof(Resource.ResourceData), ErrorMessageResourceName = "InvalidLastName")]
        [MaxLength(30)]
        public string LastName { get; set; }
        public List<int> Applications { get; set; }
        public bool IsActive { get; set; }
        public List<string> ApplicationNames { get; set; }
        public List<Role> Roles { get; set; }
        public List<Region> Regions { get; set; }

        [MaxLength(12)]
        public string DriverNo { get; set; }
        [MaxLength(50)]
        public string Email { get; set; }
        [MaxLength(15)]
        public string PhoneNumber { get; set; }
        [MaxLength(30)]
        public string PICName { get; set; }
        [MaxLength(15)]
        public string PICPhone { get; set; }
        [MaxLength(50)]
        public string PICEmail { get; set; }
        public string CreatedBy { get; set; }
    }
}
