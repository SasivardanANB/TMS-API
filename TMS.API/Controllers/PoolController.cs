using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using Helper.Model.DependencyResolver;
using System.Web.Http;
using TMS.DomainGateway.Task.Interfaces;
using TMS.DomainObjects.Request;
using TMS.DomainObjects.Response;
using TMS.DomainGateway.Gateway.Interfaces;

namespace TMS.API.Controllers
{
    [RoutePrefix("api/v1/pool")]
    public class PoolController : ApiController
    {
        [Route("createupdatepool")]
        [HttpPost]
        public IHttpActionResult CreateUpdatePool(PoolRequest poolRequest)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            IPoolTask poolTask = DependencyResolver.GetImplementationOf<ITaskGateway>().PoolTask;
            PoolResponse poolResponse = poolTask.CreateUpdatePool(poolRequest);
            return Ok(poolResponse);
        }

        [Route("deletepool")]
        [HttpDelete]
        public IHttpActionResult DeletePool(int poolID)
        {
            if (poolID <= 0)
                return BadRequest(DomainObjects.Resource.ResourceData.InvalidPool);

            IPoolTask poolTask = DependencyResolver.GetImplementationOf<ITaskGateway>().PoolTask;
            PoolResponse poolResponse = poolTask.DeletePool(poolID);
            return Ok(poolResponse);
        }

        [Route("getpools")]
        [HttpPost]
        public IHttpActionResult GetPools(PoolRequest poolRequest)
        {
            IPoolTask poolTask = DependencyResolver.GetImplementationOf<ITaskGateway>().PoolTask;
            PoolResponse poolResponse = poolTask.GetPools(poolRequest);
            return Ok(poolResponse);
        }
    }
}
