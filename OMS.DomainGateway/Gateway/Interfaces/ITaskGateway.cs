using OMS.DomainGateway.Task;
using OMS.DomainGateway.Task.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMS.DomainGateway.Gateway.Interfaces
{
    public partial interface ITaskGateway
    {
        IUserTask UserTask { get; }
        IOrderTask OrderTask { get; }
        IAuthenticateTask AuthenticateTask { get; }
    }
}
