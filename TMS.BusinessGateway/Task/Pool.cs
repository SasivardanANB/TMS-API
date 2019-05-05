using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMS.DataGateway.Repositories.Interfaces;
using TMS.DomainGateway.Task;
using TMS.DomainObjects.Request;
using TMS.DomainObjects.Response;

namespace TMS.BusinessGateway.Task
{
    public partial class BusinessPoolTask : PoolTask
    {
        private readonly IPool _poolRepository;

        public BusinessPoolTask(IPool poolRepository)
        {
            _poolRepository = poolRepository;
        }

        public override PoolResponse CreateUpdatePool(PoolRequest poolRequest)
        {
            PoolResponse poolResponse = _poolRepository.CreateUpdatePool(poolRequest);
            return poolResponse;
        }

        public override PoolResponse DeletePool(int poolID)
        {
            PoolResponse poolResponse = _poolRepository.DeletePool(poolID);
            return poolResponse;
        }

        public override PoolResponse GetPools(PoolRequest poolRequest)
        {
            PoolResponse poolResponse = _poolRepository.GetPools(poolRequest);
            return poolResponse;
        }
    }
}
