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
using TMS.API.Classes;

namespace TMS.API.Controllers
{
    [CustomAuthorize]
    [RoutePrefix("api/v1/gate")]
    public class GateController : ApiController
    {
        [Route("creategateingateout")]
        [HttpPost]
        public IHttpActionResult CreateGateInGateOut(GateRequest gateRequest)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            IGateTask gateTask = DependencyResolver.GetImplementationOf<ITaskGateway>().GateTask;
            GateResponse gateResponse = gateTask.CreateGateInGateOut(gateRequest);
            return Ok(gateResponse);
        }

        [Route("getgatelist")]
        [HttpPost]
        public IHttpActionResult GetGateList(GateRequest gateRequest)
        {
            IEnumerable<string> headerValues;
            string tokenValue = string.Empty;
            if (Request.Headers.TryGetValues("Token", out headerValues))
            {
                tokenValue = headerValues.FirstOrDefault();
                gateRequest.Token = tokenValue.ToString();
            }

            IGateTask gateTask = DependencyResolver.GetImplementationOf<ITaskGateway>().GateTask;
            GateResponse gateResponse = gateTask.GetGateList(gateRequest);
            return Ok(gateResponse);
        }
    }
}
