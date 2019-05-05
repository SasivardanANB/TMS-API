using OMS.DomainGateway.Gateway.Interfaces;
using OMS.DomainGateway.Task;
using OMS.DomainGateway.Task.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMS.DomainGateway.Gateway
{
    

    public abstract partial class TaskGateway : ITaskGateway
    {
        public abstract IUserTask UserTask { get; set; }
        public abstract IOrderTask OrderTask { get; set; }
        public abstract IAuthenticateTask AuthenticateTask {get;set;}
    }
}
