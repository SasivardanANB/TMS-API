﻿using System;
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
        public string UserName { get; set; }
        [Required(ErrorMessageResourceType = typeof(Resource.ResourceData), ErrorMessageResourceName = "InvalidUserPassword")]
        [MinLength(8)]
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string PICName { get; set; }
        public string PICPhone { get; set; }
        public string PICEmail { get; set; }
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
    }
}
