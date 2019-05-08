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
    [RoutePrefix("api/v1/driver")]
    public class DriverController : ApiController
    {
        [Route("createupdatedriver")]
        [HttpPost]
        public IHttpActionResult CreateUpdateDriver(DriverRequest driverRequest)
        {
            //For removing validation errors for password, confirmpassword in case of update
            for (int i = 0; i < driverRequest.Requests.Count; i++)
            {
                if (driverRequest.Requests[i].ID > 0)
                {
                    ModelState.Remove("driverRequest.Requests[" + i + "].Password");
                    ModelState.Remove("driverRequest.Requests[" + i + "].ConfirmPassword");
                }
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            IDriverTask driverTask = DependencyResolver.GetImplementationOf<ITaskGateway>().DriverTask;
            DriverResponse driverResponse = driverTask.CreateUpdateDriver(driverRequest);
            return Ok(driverResponse);
        }

        [Route("deletedriver")]
        [HttpDelete]
        public IHttpActionResult DeleteDriver(int driverID)
        {
            if (driverID <= 0)
                return BadRequest(DomainObjects.Resource.ResourceData.InvalidDriver);

            IDriverTask driverTask = DependencyResolver.GetImplementationOf<ITaskGateway>().DriverTask;
            DriverResponse driverResponse = driverTask.DeleteDriver(driverID);
            return Ok(driverResponse);
        }

        [Route("getdrivers")]
        [HttpPost]
        public IHttpActionResult GetDrivers(DriverRequest driverRequest)
        {
            IDriverTask driverTask = DependencyResolver.GetImplementationOf<ITaskGateway>().DriverTask;
            DriverResponse driverResponse = driverTask.GetDrivers(driverRequest);
            return Ok(driverResponse);
        }
    }
}
