using OMS.DomainGateway.Task.Interfaces;

namespace OMS.DomainGateway.Gateway.Interfaces
{
    public partial interface ITaskGateway
    {
        IUserTask UserTask { get; }
        IOrderTask OrderTask { get; }
        IAuthenticateTask AuthenticateTask { get; }
        IMasterTask MasterTask { get; }
    }
}
