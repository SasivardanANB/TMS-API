﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMS.DomainObjects.Objects
{
    public class Authenticate
    {
        public int TokenID { get; set; }
        [MaxLength(200)]
        public string TokenKey { get; set; }
        public DateTime IssuedOn { get; set; }
        public DateTime ExpiresOn { get; set; }
        public DateTime CreatedOn { get; set; }
        public int UserID { get; set; }
    }
}
