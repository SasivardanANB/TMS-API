using DMS.DomainGateway.Task;
using DMS.DomainGateway.Task.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMS.DomainGateway.Gateway.Interfaces
{
    public partial interface ITaskGateway
    {
        IUserTask UserTask { get; }
        IAuthenticateTask AuthenticateTask { get; }
        ITripTask TripTask { get; }
        IMediaTask MediaTask { get; }
        IMasterTask MasterTask { get; }
    }
}
