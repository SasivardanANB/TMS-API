using OMS.DomainObjects.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMS.DomainObjects.Request
{
    public class RoleRequest : RequestFilter
    {
        public List<Role> Requests { get; set; }
    }
}
