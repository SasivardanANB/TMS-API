using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMS.DomainObjects.Objects
{
    public class Driver
    {
        public int ID { get; set; }
        //[Required(ErrorMessageResourceType = typeof(Resource.ResourceData), ErrorMessageResourceName = "InvalidTransporterId")]
        //[Range(minimum: 1, maximum: int.MaxValue, ErrorMessageResourceType = typeof(Resource.ResourceData), ErrorMessageResourceName = "InvalidTransporterId")]
        public int TransporterId { get; set; }
        public string TransporterName { get; set; }
        //[Required(ErrorMessageResourceType = typeof(Resource.ResourceData), ErrorMessageResourceName = "InvalidDriverNo")]
        [MaxLength(12)]
        public string DriverNo { get; set; }
        [MaxLength(30)]
        [Required(ErrorMessageResourceType = typeof(Resource.ResourceData), ErrorMessageResourceName = "InvalidUserName")]
        public string UserName { get; set; }
        [MaxLength(30)]
        [Required(ErrorMessageResourceType = typeof(Resource.ResourceData), ErrorMessageResourceName = "InvalidFirstName")]
        public string FirstName { get; set; }
        [MaxLength(30)]
        [Required(ErrorMessageResourceType = typeof(Resource.ResourceData), ErrorMessageResourceName = "InvalidLastName")]
        public string LastName { get; set; }
        [MaxLength(200)]
        [Required(ErrorMessageResourceType = typeof(Resource.ResourceData), ErrorMessageResourceName = "InvalidAddress")]        
        public string DriverAddress { get; set; }
        [MaxLength(15)]
        [Required(ErrorMessageResourceType = typeof(Resource.ResourceData), ErrorMessageResourceName = "InvalidPhone")]
        public string DriverPhone { get; set; }
        public bool IsActive { get; set; }
        [MaxLength(50)]
        [Required(ErrorMessageResourceType = typeof(Resource.ResourceData), ErrorMessageResourceName = "InvalidEmail")]
        public string Email { get; set; }
        [DataType(DataType.Password)]
        [MaxLength(30)]
        [Required(ErrorMessageResourceType = typeof(Resource.ResourceData), ErrorMessageResourceName = "PasswordInvalid")]
        [RegularExpression("^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{8,}$", ErrorMessageResourceType = typeof(Resource.ResourceData), ErrorMessageResourceName = "PasswordInvalid")]
        public string Password { get; set; }
        //[DataType(DataType.Password)]
        //[Required(ErrorMessageResourceType = typeof(Resource.ResourceData), ErrorMessageResourceName = "InvalidConfirmPassword")]
        //[Compare("Password", ErrorMessageResourceType = typeof(Resource.ResourceData), ErrorMessageResourceName = "PasswordMismatch")]
        //public string ConfirmPassword { get; set; }
        [MaxLength(20)]
        [Required(ErrorMessageResourceType = typeof(Resource.ResourceData), ErrorMessageResourceName = "InvalidIdentityNo")]
        public string IdentityNo { get; set; }
        [MaxLength(20)]
        [Required(ErrorMessageResourceType = typeof(Resource.ResourceData), ErrorMessageResourceName = "InvalidDrivingLicenseNo")]
        public string DrivingLicenseNo { get; set; }
        [Required(ErrorMessageResourceType = typeof(Resource.ResourceData), ErrorMessageResourceName = "InvalidExpireDate")]
        public DateTime? DrivingLicenseExpiredDate { get; set; }
        public bool IsDelete { get; set; }
        public int IdentityImageId { get; set; }
        [Required(ErrorMessageResourceType = typeof(Resource.ResourceData), ErrorMessageResourceName = "InvalidIdentityImage")]
        public string IdentityImageGuId { get; set; }
        public int DrivingLicenceImageId { get; set; }
        [Required(ErrorMessageResourceType = typeof(Resource.ResourceData), ErrorMessageResourceName = "InvalidDrivingLicenseImage")]
        public string DrivingLicenceImageGuId { get; set; }
        public int DriverImageId { get; set; }
        [Required(ErrorMessageResourceType = typeof(Resource.ResourceData), ErrorMessageResourceName = "InvalidImage")]
        public string DriverImageGuId { get; set; }
    }
}
