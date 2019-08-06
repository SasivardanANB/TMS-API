using TMS.DataGateway.Repositories.Iterfaces;
using TMS.DomainGateway.Task;
using System;

namespace TMS.BusinessGateway.Task
{
    public partial class BusinessAuthenticateTask : AuthenticateTask
    {
        private readonly IAuthenticate _userRepository;
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
    }
}
