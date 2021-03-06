﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMS.DataGateway.DataModels
{
    [Table("Driver", Schema = "TMS")]
    public class Driver : ModelBase
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [ForeignKey("Partner")]
        public int? TransporterId { get; set; }
        public virtual Partner Partner { get; set; }
        [Index("Driver_DriverNo", IsUnique = true)]
        [MaxLength(12)]
        public string DriverNo { get; set; }
        [MaxLength(30)]
        [Required()]
        public string UserName { get; set; }
        [MaxLength(30)]
        [Required(ErrorMessage = "Enter first name")]
        public string FirstName { get; set; }
        [MaxLength(30)]
        [Required(ErrorMessage = "Enter last name")]
        public string LastName { get; set; }
        [MaxLength(200)]
        public string DriverAddress { get; set; }
        [MaxLength(15)]
        [Required(ErrorMessage = "Enter driver phone no")]
        public string DriverPhone { get; set; }
        public bool IsActive { get; set; }
        [MaxLength(50)]
        public string Email { get; set; }
        [MaxLength(30)]
        [Required(ErrorMessage = "Enter password")]
        public string Password { get; set; }
        [MaxLength(25)]
        [Index("Driver_IdentityNo", IsUnique = true)]
        public string IdentityNo { get; set; }
        [MaxLength(25)]
        [Index("Driver_DrivingLicenseNo", IsUnique = true)]
        public string DrivingLicenseNo { get; set; }
        public DateTime? DrivingLicenseExpiredDate { get; set; }
        [ForeignKey("ImageGuid")]
        public int IdentityImageId { get; set; }
        public int DrivingLicenceImageId { get; set; }
        public int DriverImageId { get; set; }
        public virtual ImageGuid ImageGuid { get; set; }
        public bool IsDelete { get; set; }
        
    }
}
