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
    [RoutePrefix("api/v1/master")]
    public class MasterController : ApiController
    {
        [Route("getpartners")]
        [HttpPost]
        public IHttpActionResult GetPartners(PartnerSearchRequest partnerSearchRequest)
        {
            IMasterTask masterTask = Helper.Model.DependencyResolver.DependencyResolver.GetImplementationOf<ITaskGateway>().MasterTask;
            PartnerSearchResponse partnerSearchResponse = masterTask.GetPartners(partnerSearchRequest);
            return Ok(partnerSearchResponse);
        }

        [Route("getdrivernames")]
        [HttpPost]
        public IHttpActionResult GetDriverNames()
        {
            IMasterTask masterTask = Helper.Model.DependencyResolver.DependencyResolver.GetImplementationOf<ITaskGateway>().MasterTask;
            CommonResponse commonResponse = masterTask.GetDriverNames();
            return Ok(commonResponse);
        }

        [Route("getvehicletypenames")]
        [HttpPost]
        public IHttpActionResult GetVehicleTypeNames()
        {
            IMasterTask masterTask = Helper.Model.DependencyResolver.DependencyResolver.GetImplementationOf<ITaskGateway>().MasterTask;
            CommonResponse commonResponse = masterTask.GetDriverNames();
            return Ok(commonResponse);
        }
    }
}
