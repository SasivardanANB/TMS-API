﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMS.DomainObjects.Objects;

namespace TMS.DomainObjects.Response
{
    public class UserRoleResponse : Message
    {
        public List<UserRole> Data { get; set; }
    }
}
