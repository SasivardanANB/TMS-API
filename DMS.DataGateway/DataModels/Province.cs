﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
//using CsvHelper.Configuration.Attributes;

namespace DMS.DataGateway.DataModels
{
    [Table("Province", Schema = "DMS")]
    public class Province : ModelBase
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [MaxLength(4)]
        [Index("Province_ProvinceCode", IsUnique = true)]
        public string ProvinceCode { get; set; }
        [MaxLength(50)]
        public string ProvinceDescription { get; set; }
       
    }
}
