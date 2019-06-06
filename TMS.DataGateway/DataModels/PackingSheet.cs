﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMS.DataGateway.DataModels
{
    [Table("PackingSheet", Schema = "TMS")]
    public class PackingSheet : ModelBase
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        //[ForeignKey("OrderDetail")]
        //public int OrderDetailID { get; set; }
        //public OrderDetail OrderDetail { get; set; }
        [MaxLength(20)]
        public string ShippingListNo { get; set; }
        [MaxLength(50)]
        public string PackingSheetNo { get; set; }
        
    }
}
