using OMS.DomainGateway.Gateway.Interfaces;
using OMS.DomainGateway.Task.Interfaces;

namespace OMS.DomainGateway.Gateway
{
    public abstract partial class TaskGateway : ITaskGateway
    {
        public abstract IUserTask UserTask { get; set; }
        public abstract IOrderTask OrderTask { get; set; }
        public abstract IAuthenticateTask AuthenticateTask {get;set;}
        public abstract IMasterTask MasterTask {get;set;}
    }
}
