using DMS.BusinessGateway.Task;
using DMS.DataGateway.Repositories;
using DMS.DomainGateway.Gateway;
using DMS.DomainGateway.Gateway.Interfaces;
using DMS.DomainGateway.Task;
using DMS.DomainGateway.Task.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMS.BusinessGateway.Gateway
{
    public class TaskGateway : ITaskGateway
    {
        public IUserTask UserTask
        {
            get { return new BusinessUserTask(new User()); }
        }
        
        public IAuthenticateTask AuthenticateTask
        {
            get { return new BusinessAuthenticateTask(new Authenticate()); }
        }

        public ITripTask TripTask
        {
            get { return new BusinessTripTask(new Trip()); }
        }

        public IMediaTask MediaTask
        {
            get { return new BusinessMediaTask(); }
        }
        public IMasterTask MasterTask
        {
            get { return new BusinessMasterTask(new Master()); }
        }
    }
}
