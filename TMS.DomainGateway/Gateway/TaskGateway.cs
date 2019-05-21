using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMS.DomainGateway.Gateway.Interfaces;
using TMS.DomainGateway.Task.Interfaces;

namespace TMS.DomainGateway.Gateway
{
    public abstract partial class TaskGateway: ITaskGateway
    {
        public abstract IDriverTask DriverTask { get; set; }
        public abstract IUserTask UserTask { get; set; }
        public abstract IAuthenticateTask AuthenticateTask { get; set; }
        public abstract IVehicleTask VehicleTask { get; }
        public abstract IPoolTask PoolTask { get; }
        public abstract IPICTask PICTask { get; set; }
        public abstract IPartnerTask PartnerTask { get; set; }
        public abstract IOrderTask OrderTask { get; set; }
        public abstract IMasterTask MasterTask { get; set; }
        public abstract IGateTask GateTask { get; set; }
    }
}
