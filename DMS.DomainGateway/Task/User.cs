using DMS.DomainGateway.Task.Interfaces;
using DMS.DomainObjects.Request;
using DMS.DomainObjects.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMS.DomainGateway.Task
{
    
    public abstract class UserTask : IUserTask
    {
        public abstract UserResponse LoginUser(LoginRequest login);
        public abstract UserResponse CreateUpdateUser(UserRequest user);
        public abstract UserResponse GetProfileDetails(int userID);
        public abstract UserResponse ChangePassword(ChangePasswordRequest changepassword, string type);
        public abstract UserResponse ForgotPassword(ForgotPasswordRequest forgotPassword);

    }
}
