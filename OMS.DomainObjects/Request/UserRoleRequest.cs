﻿using OMS.DomainObjects.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMS.DomainObjects.Request
{
    public class UserRoleRequest : RequestFilter
    {
        public List<UserRole> Requests { get; set; }
    }
}
