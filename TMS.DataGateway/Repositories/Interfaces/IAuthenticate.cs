using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain = TMS.DomainObjects.Objects;

namespace TMS.DataGateway.Repositories.Iterfaces
{
    public interface IAuthenticate
    {
        int InsertToken(Domain.Authenticate token);
        string GenerateToken(Domain.User user, DateTime IssuedOn);
        bool ValidateToken(string token);
    }
}
