using DMS.DomainObjects.Request;
using DMS.DomainObjects.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain = DMS.DomainObjects.Objects;

namespace DMS.DataGateway.Repositories.Iterfaces
{
    public interface IUser
    {
        UserResponse LoginUser(LoginRequest login);
        UserResponse CreateUpdateUser(UserRequest user);
    }
}
