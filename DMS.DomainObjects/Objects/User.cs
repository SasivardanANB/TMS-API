using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace DMS.DomainObjects.Objects
{
    public class User
    {
        public int ID { get; set; }
        [MaxLength(12)]
        public string DriverNo { get; set; }
        [Required(ErrorMessageResourceType = typeof(Resource.ResourceData), ErrorMessageResourceName = "InvalidUserName")]
        [MaxLength(30)]
        public string UserName { get; set; }
        [Required(ErrorMessageResourceType = typeof(Resource.ResourceData), ErrorMessageResourceName = "InvalidUserPassword")]
        [MaxLength(30)]
        public string Password { get; set; }
        [MaxLength(30)]
        public string FirstName { get; set; }
        [MaxLength(30)]
        public string LastName { get; set; }
        [MaxLength(50)]
        public string Email { get; set; }
        [MaxLength(15)]
        public string PhoneNumber { get; set; }
        [MaxLength(60)]
        public string PICName { get; set; }
        [MaxLength(15)]
        public string PICPhone { get; set; }
        [MaxLength(50)]
        public string PICEmail { get; set; }
        public bool IsActive { get; set; }
        [MaxLength(100)]
        public string CreatedBy { get; set; }
        public string Role { get; set; }
    }
}
