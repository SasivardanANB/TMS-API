using OMS.DomainGateway.Task.Interfaces;
using OMS.DomainObjects.Objects;
using OMS.DomainObjects.Request;
using OMS.DomainObjects.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMS.DomainGateway.Task
{
    public abstract class AuthenticateTask : IAuthenticateTask
    {
        public abstract int InsertToken(Authenticate token);
        public abstract string GenerateToken(User user, DateTime IssuedOn);
        public abstract bool ValidateToken(string token);
    }
}
