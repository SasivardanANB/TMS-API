﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMS.DomainGateway.Task.Interfaces;

namespace TMS.DomainGateway.Gateway.Interfaces
{
    public partial interface ITaskGateway
    {
        IDriverTask DriverTask { get; }
        IUserTask UserTask { get; }
        IAuthenticateTask AuthenticateTask { get; }
        ITransporterTask TransporterTask { get; }
        IVehicleTask VehicleTask { get; }
        IPoolTask PoolTask { get; }
        IPICTask PICTask { get; }
        IPartnerTask PartnerTask { get; }
        IOrderTask OrderTask { get; }
    }
}
