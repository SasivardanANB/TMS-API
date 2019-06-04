﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMS.DomainObjects.Objects
{
    public class OrderStatus
    {
        public int OrderType { get; set; }
        [MaxLength(15)]
        public string OrderNumber { get; set; }
        public int SequenceNumber { get; set; }
        public bool? IsLoad { get; set; }
        [MaxLength(50)]
        public string Remarks { get; set; }
        [MaxLength(4)]
        public string OrderStatusCode { get; set; }
    }
}
