using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TMS.API.Classes;
using TMS.DomainGateway.Gateway.Interfaces;
using TMS.DomainGateway.Task.Interfaces;
using TMS.DomainObjects.Request;
using TMS.DomainObjects.Response;


namespace TMS.API.Controllers
{
    [CustomAuthorize]
    [RoutePrefix("api/v1/pic")]
    public class PICController : ApiController
    {
        [Route("createupdatepic")]
        [HttpPost]
        public IHttpActionResult CreateUpdatePIC(PICRequest picRequest)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            IPICTask picTask = Helper.Model.DependencyResolver.DependencyResolver.GetImplementationOf<ITaskGateway>().PICTask;
            PICResponse picResponse = picTask.CreateUpdatePIC(picRequest);
            return Ok(picResponse);
        }

        [Route("deletepic")]
        [HttpDelete]
        public IHttpActionResult DeletePIC(int picId)
        {
            if (picId > 0)
            {
                IPICTask picTask = Helper.Model.DependencyResolver.DependencyResolver.GetImplementationOf<ITaskGateway>().PICTask;
                PICResponse picResponse = picTask.DeletePIC(picId);
                return Ok(picResponse);
            }
            else
            {
                return BadRequest();
            }
        }

        [Route("getpics")]
        [HttpPost]
        public IHttpActionResult GetPICs(PICRequest picRequest)
        {
            IPICTask picTask = Helper.Model.DependencyResolver.DependencyResolver.GetImplementationOf<ITaskGateway>().PICTask;
            PICResponse transporterResponse = picTask.GetPICs(picRequest);
            return Ok(transporterResponse);
        }

    }
}
