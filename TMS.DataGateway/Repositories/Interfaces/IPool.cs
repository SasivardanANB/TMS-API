using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMS.DomainObjects.Request;
using TMS.DomainObjects.Response;

namespace TMS.DataGateway.Repositories.Interfaces
{
    public interface IPool
    {
        PoolResponse CreateUpdatePool(PoolRequest poolRequest);
        PoolResponse DeletePool(int poolID);
        PoolResponse GetPools(PoolRequest poolRequest);
    }
}
