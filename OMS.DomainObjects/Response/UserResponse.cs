using OMS.DomainObjects.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMS.DomainObjects.Response
{
    public class UserResponse : Message
    {
        public List<User> Data { get; set; }
        public List<Role> RoleData { get; set; }
    }
}
