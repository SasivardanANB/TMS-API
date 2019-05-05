using OMS.BusinessGateway.Task;
using OMS.DataGateway.Repositories;
using OMS.DomainGateway.Gateway;
using OMS.DomainGateway.Gateway.Interfaces;
using OMS.DomainGateway.Task;
using OMS.DomainGateway.Task.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMS.BusinessGateway.Gateway
{
    public class TaskGateway : ITaskGateway
    {
        public IUserTask UserTask
        {
            get { return new BusinessUserTask(new User()); }
        }
        public IOrderTask OrderTask
        {
            get { return new BusinessOrderTask(new Order()); }
        }
        public IAuthenticateTask AuthenticateTask
        {
            get { return new BusinessAuthenticateTask(new Authenticate()); }
        }
    }
}
