using DMS.DataGateway.Repositories;
using DMS.DataGateway.Repositories.Iterfaces;
using DMS.DomainGateway.Task;
using DMS.DomainObjects.Objects;
using DMS.DomainObjects.Request;
using DMS.DomainObjects.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMS.BusinessGateway.Task
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
    }
}
