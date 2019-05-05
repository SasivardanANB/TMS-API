using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMS.DomainGateway.Task.Interfaces;
using TMS.DomainObjects.Request;
using TMS.DomainObjects.Response;

namespace TMS.DomainGateway.Task
{
    public abstract class PoolTask : IPoolTask
    {
        public abstract PoolResponse CreateUpdatePool(PoolRequest poolRequest);
        public abstract PoolResponse DeletePool(int poolID);
        public abstract PoolResponse GetPools(PoolRequest poolRequest);
    }
}
