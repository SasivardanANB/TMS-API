﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMS.DomainObjects.Objects
{
    public class PostalCode
    {
        public int ID { get; set; }
        public string PostalCodeNo { get; set; }
        public int SubDistrictID { get; set; }
    }
}
