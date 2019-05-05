using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TMS.DomainGateway.Gateway.Interfaces;
using TMS.DomainGateway.Task.Interfaces;
using TMS.DomainObjects.Request;
using TMS.DomainObjects.Response;

namespace TMS.API.Controllers
{
    [RoutePrefix("api/v1/transporter")]
    public class TransporterController : ApiController
    {
        [Route("createupdatetransporter")]
        [HttpPost]
        public IHttpActionResult CreateUpdateTransporter(TransporterRequest transporterRequest)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            ITransporterTask transporterTask = Helper.Model.DependencyResolver.DependencyResolver.GetImplementationOf<ITaskGateway>().TransporterTask;
            TransporterResponse transporterResponse = transporterTask.CreateUpdateTransporter(transporterRequest);
            return Ok(transporterResponse);
        }

        [Route("deletetransporter")]
        [HttpDelete]
        public IHttpActionResult DeleteTransporter(int transporterId)
        {
            if (transporterId > 0)
            {
                ITransporterTask transporterTask = Helper.Model.DependencyResolver.DependencyResolver.GetImplementationOf<ITaskGateway>().TransporterTask;
                TransporterResponse transporterResponse = transporterTask.DeleteTransporter(transporterId);
                return Ok(transporterResponse);
            }
            else
            {
                return BadRequest();
            }
        }

        [Route("gettransporters")]
        [HttpPost]
        public IHttpActionResult GetTransporters(TransporterRequest transporterRequest)
        {
            ITransporterTask transporterTask = Helper.Model.DependencyResolver.DependencyResolver.GetImplementationOf<ITaskGateway>().TransporterTask;
            TransporterResponse transporterResponse = transporterTask.GetTransporters(transporterRequest);
            return Ok(transporterResponse);
        }
    }
}
