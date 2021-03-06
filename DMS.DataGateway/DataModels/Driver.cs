﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMS.DataGateway.DataModels
{
    [Table("Driver", Schema = "DMS")]
    public class Driver : ModelBase
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [Index("Driver_DriverNo", IsUnique = true)]
        [MaxLength(12)]
        public string DriverNo { get; set; }
        [Required(ErrorMessage = "")]
        [MaxLength(30)]
        public string UserName { get; set; }
        [MaxLength(30)]
        public string Password { get; set; }
        public bool IsActive { get; set; }
        [MaxLength(30)]
        public string FirstName { get; set; }
        [MaxLength(30)]
        public string LastName { get; set; }
        [MaxLength(50)]
        public string Email { get; set; }
        [MaxLength(15)]
        public string PhoneNumber { get; set; }
        [MaxLength(50)]
        public string PICName { get; set; }
        [MaxLength(15)]
        public string PICPhone { get; set; }
        [MaxLength(50)]
        public string PICEmail { get; set; }
        
    }
}
