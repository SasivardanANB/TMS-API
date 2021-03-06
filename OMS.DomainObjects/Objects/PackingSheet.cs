﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace OMS.DomainObjects.Objects
{
    public class PackingSheet
    {
        [MaxLength(15)]
        public string OrderNumber { get; set; }
        [MaxLength(200)]
        public string Notes { get; set; }
        public int DealerId { get; set; }
        public string DealerNumber { get; set; }
        public int OrderDetailId { get; set; }
        [MaxLength(20)]
        public string ShippingListNo { get; set; }
        public int Collie { get; set; }
        public string Katerangan { get; set; }
        public List<Common> PackingSheetNumbers { get; set; }
    }
}
