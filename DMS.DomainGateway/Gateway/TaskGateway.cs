using DMS.DomainGateway.Gateway.Interfaces;
using DMS.DomainGateway.Task.Interfaces;

namespace DMS.DomainGateway.Gateway
{
    public abstract partial class TaskGateway : ITaskGateway
    {
        public abstract IUserTask UserTask { get; set; }
        public abstract IAuthenticateTask AuthenticateTask {get;set;}
        public abstract ITripTask TripTask { get; set; }
        public abstract IMediaTask MediaTask { get; set; }
        public abstract IMasterTask MasterTask { get; set; }
    }
}
