using DMS.DomainObjects.Request;
using DMS.DomainObjects.Response;
using System.Net.Http;

namespace DMS.DomainGateway.Task.Interfaces
{
    public interface IUserTask
    {
        UserResponse LoginUser(LoginRequest login);
        UserResponse CreateUpdateUser(UserRequest user);
        UserResponse ChangePassword(ChangePasswordRequest changePasswordRequest,string type);
        UserResponse GetProfileDetails(int userID);
        HttpResponseMessage ForgotPassword(ForgotPasswordRequest forgotPasswordRequest);
    }
}
