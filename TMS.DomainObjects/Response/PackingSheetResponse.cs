﻿using TMS.DomainObjects.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMS.DomainObjects.Response
{
    public class PackingSheetResponse : Message
    {
        public List<PackingSheet> Data { get; set; }
    }
}
