﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMS.DataGateway.DataModels
{
    [Table("RoleMenuActivity", Schema = "OMS")]
    public class RoleMenuActivity : ModelBase
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [ForeignKey("RoleMenu")]
        public int RoleMenuID { get; set; }
        public RoleMenu RoleMenu { get; set; }
        [ForeignKey("Activity")]
        public int ActivityID { get; set; }
        public Activity Activity { get; set; }
       

    }
}
