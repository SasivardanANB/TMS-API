﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMS.DomainObjects.Objects;

namespace TMS.DomainObjects.Response
{
    public class OrderStatusResponse : Message
    {
        public List<OrderStatus> Data { get; set; }
    }
}
