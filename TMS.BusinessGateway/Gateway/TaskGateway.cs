using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMS.BusinessGateway.Task;
using TMS.DataGateway.Repositories;
using TMS.DomainGateway.Gateway.Interfaces;
using TMS.DomainGateway.Task.Interfaces;

namespace TMS.BusinessGateway.Gateway
{
    public class TaskGateway: ITaskGateway
    {
        public IDriverTask DriverTask
        {
            get { return new BusinessDriverTask(new Driver()); }
        }
        public IUserTask UserTask
        {
            get { return new BusinessUserTask(new User()); }
        }
        public IAuthenticateTask AuthenticateTask
        {
            get { return new BusinessAuthenticateTask(new Authenticate()); }
        }
        public IOrderTask OrderTask
        {
            get { return new BusinessOrderTask(new Order()); }
        }

        public IVehicleTask VehicleTask
        {
            get { return new BusinessVehicleTask(new Vehicle()); }
        }

        public IPoolTask PoolTask
        {
            get { return new BusinessPoolTask(new Pool()); }
        }

        public IPICTask PICTask
        {
            get { return new BusinessPICTask(new PIC()); }
        }

        public IPartnerTask PartnerTask
        {
            get { return new BusinessPartnerTask(new Partner()); }
        }

        public IMasterTask MasterTask
        {
            get { return new BusinessMasterTask(new Master()); }
        }

        public IGateTask GateTask
        {
            get { return new BusinessGateTask(new Gate()); }
        }
    }
}
