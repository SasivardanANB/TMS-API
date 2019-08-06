using DMS.DomainObjects.Objects;
using System;

namespace DMS.DomainGateway.Task.Interfaces
{
    public interface IAuthenticateTask
    {
        int InsertToken(Authenticate token);
        string GenerateToken(User user, DateTime IssuedOn);
        bool ValidateToken(string token);
    }
}
