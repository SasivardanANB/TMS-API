﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMS.DataGateway.DataModels
{
    [Table("MenuActivity", Schema = "TMS")]
    public class MenuActivity : ModelBase
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [ForeignKey("Menu")]
        public int MenuID { get; set; }
        public Menu Menu { get; set; }
        [ForeignKey("Activity")]
        public int ActivityID { get; set; }
        public Activity Activity { get; set; }
       
    }
}
