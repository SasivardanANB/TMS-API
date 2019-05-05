using TMS.DomainObjects.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMS.DomainObjects.Request
{
    public class UserRequest : RequestFilter
    {
        public List<User> Requests { get; set; }

    }
}
