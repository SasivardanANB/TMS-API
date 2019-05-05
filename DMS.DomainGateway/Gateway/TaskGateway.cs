using DMS.DomainGateway.Gateway.Interfaces;
using DMS.DomainGateway.Task;
using DMS.DomainGateway.Task.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMS.DomainGateway.Gateway
{
    public abstract partial class TaskGateway : ITaskGateway
    {
        public abstract IUserTask UserTask { get; set; }
        public abstract IAuthenticateTask AuthenticateTask {get;set;}
        public abstract ITripTask TripTask { get; set; }
    }
}
