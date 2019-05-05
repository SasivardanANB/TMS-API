using DMS.DomainGateway.Task.Interfaces;
using DMS.DomainObjects.Objects;
using DMS.DomainObjects.Request;
using DMS.DomainObjects.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMS.DomainGateway.Task
{
    public abstract class AuthenticateTask : IAuthenticateTask
    {
        public abstract int InsertToken(Authenticate token);
        public abstract string GenerateToken(User user, DateTime IssuedOn);
        public abstract bool ValidateToken(string token);
    }
}
