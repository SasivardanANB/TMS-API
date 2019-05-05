using TMS.DomainGateway.Task.Interfaces;
using TMS.DomainObjects.Objects;
using TMS.DomainObjects.Request;
using TMS.DomainObjects.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMS.DomainGateway.Task
{
    public abstract class AuthenticateTask : IAuthenticateTask
    {
        public abstract int InsertToken(Authenticate token);
        public abstract string GenerateToken(User user, DateTime IssuedOn);
        public abstract bool ValidateToken(string token);
    }
}
