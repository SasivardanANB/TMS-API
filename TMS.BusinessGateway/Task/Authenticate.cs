using TMS.DataGateway.Repositories;
using TMS.DataGateway.Repositories.Iterfaces;
using TMS.DomainGateway.Task;
using TMS.DomainObjects.Objects;
using TMS.DomainObjects.Request;
using TMS.DomainObjects.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMS.BusinessGateway.Task
{
    public partial class BusinessAuthenticateTask : AuthenticateTask
    {
        private IAuthenticate _userRepository;
        public BusinessAuthenticateTask(IAuthenticate userRepository)
        {
            _userRepository = userRepository;
        }
        public override string GenerateToken(DomainObjects.Objects.User user, DateTime IssuedOn)
        {
            string token = _userRepository.GenerateToken(user, IssuedOn);
            return token;
        }

        public override int InsertToken(DomainObjects.Objects.Authenticate token)
        {
            int res = _userRepository.InsertToken(token);
            return res;
        }

        public override bool ValidateToken(string token)
        {
            bool res = _userRepository.ValidateToken(token);
            return res;
        }

        //public override UserResponse LoginUser(LoginRequest login)
        //{
        //    //If needed write business logic here for request.

        //    UserResponse userData = _userRepository.LoginUser(login);

        //    //If needed write business logic here for response.
        //    return userData;
        //}

        //public override UserResponse CreateUser(UserRequest user)
        //{
        //    //If needed write business logic here for request.

        //    UserResponse userData = _userRepository.CreateUser(user);

        //    //If needed write business logic here for response.
        //    return userData;
        //}
    }
}
