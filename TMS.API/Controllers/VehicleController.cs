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
    [RoutePrefix("api/v1/vehicle")]
    public class VehicleController : ApiController
    {
        [Route("createupdatevehicle")]
        [HttpPost]
        public IHttpActionResult CreateUpdateVehicle(VehicleRequest vehicleRequest)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            IVehicleTask vehicleTask = DependencyResolver.GetImplementationOf<ITaskGateway>().VehicleTask;
            VehicleResponse vehicleResponse = vehicleTask.CreateUpdateVehicle(vehicleRequest);
            return Ok(vehicleResponse);
        }

        [Route("deletevehicle")]
        [HttpDelete]
        public IHttpActionResult DeleteVehicle(int vehicleID)
        {
            if (vehicleID <= 0)
                return BadRequest(DomainObjects.Resource.ResourceData.InvalidVehicle);

            IVehicleTask vehicleTask = DependencyResolver.GetImplementationOf<ITaskGateway>().VehicleTask;
            VehicleResponse vehicleResponse = vehicleTask.DeleteVehicle(vehicleID);
            return Ok(vehicleResponse);
        }

        [Route("getvehicles")]
        [HttpPost]
        public IHttpActionResult GetVehicles(VehicleRequest vehicleRequest)
        {
            IVehicleTask vehicleTask = DependencyResolver.GetImplementationOf<ITaskGateway>().VehicleTask;
            VehicleResponse vehicleResponse = vehicleTask.GetVehicles(vehicleRequest);
            return Ok(vehicleResponse);
        }
    }
}
