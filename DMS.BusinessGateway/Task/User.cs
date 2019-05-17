using DMS.DataGateway.Repositories;
using DMS.DataGateway.Repositories.Iterfaces;
using DMS.DomainGateway.Task;
using DMS.DomainObjects.Request;
using DMS.DomainObjects.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMS.BusinessGateway.Task
{
    public partial class BusinessUserTask : UserTask
    {
        private IUser _userRepository;
        public BusinessUserTask(IUser userRepository)
        {
            _userRepository = userRepository;
        }

        public override UserResponse LoginUser(LoginRequest login)
        {
            //If needed write business logic here for request.

            UserResponse userData = _userRepository.LoginUser(login);

            //If needed write business logic here for response.
            return userData;
        }

        public override UserResponse CreateUpdateUser(UserRequest user)
        {
            //If needed write business logic here for request.

            UserResponse userData = _userRepository.CreateUpdateUser(user);

            //If needed write business logic here for response.
            return userData;
        }
        public override UserResponse ChangePassword(ChangePasswordRequest changePasswordRequest)
        {
            UserResponse userData = _userRepository.ChangePassword(changePasswordRequest);
            return userData;
        }
        public override UserResponse ForgotPassword(ForgotPasswordRequest forgotPasswordRequest)
        {
            UserResponse userData = _userRepository.ForgotPassword(forgotPasswordRequest);
            return userData;
        }
        
    }
}
