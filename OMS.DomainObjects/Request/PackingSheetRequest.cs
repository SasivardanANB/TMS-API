﻿using OMS.DomainObjects.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMS.DomainObjects.Request
{
    public class PackingSheetRequest : RequestFilter
    {
        public List<PackingSheet> Requests { get; set; }
    }
}
