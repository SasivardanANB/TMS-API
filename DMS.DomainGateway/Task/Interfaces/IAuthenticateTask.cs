using DMS.DomainObjects.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMS.DomainGateway.Task.Interfaces
{
    public interface IAuthenticateTask
    {
        int InsertToken(Authenticate token);
        string GenerateToken(User user, DateTime IssuedOn);
        bool ValidateToken(string token);
    }
}
