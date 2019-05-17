using DMS.DomainObjects.Request;
using DMS.DomainObjects.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMS.DomainGateway.Task.Interfaces
{
    public interface IUserTask
    {
        UserResponse LoginUser(LoginRequest login);
        UserResponse CreateUpdateUser(UserRequest user);
        UserResponse ChangePassword(ChangePasswordRequest changePasswordRequest);
        UserResponse ForgotPassword(ForgotPasswordRequest forgotPasswordRequest);
        UserResponse GetProfileDetails(int userID);

    }
}
