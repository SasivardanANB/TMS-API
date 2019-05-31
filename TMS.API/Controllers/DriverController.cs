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
using RestSharp;
using System.Configuration;
using Newtonsoft.Json;
using TMS.DomainObjects.Objects;

namespace TMS.API.Controllers
{
    [CustomAuthorize]
    [RoutePrefix("api/v1/driver")]
    public class DriverController : ApiController
    {
        #region Private Methods
        private static string GetApiResponse(string apiRoute, Method method, object requestQueryParameter, string token)
        {
            var client = new RestClient(ConfigurationManager.AppSettings["ApiGatewayBaseURL"]);
            client.AddDefaultHeader("Content-Type", "application/json");
            if (token != null)
                client.AddDefaultHeader("Token", token);
            var request = new RestRequest(apiRoute, method) { RequestFormat = DataFormat.Json };
            request.Timeout = 500000;
            if (requestQueryParameter != null)
            {
                request.AddJsonBody(requestQueryParameter);
            }
            var result = client.Execute(request);
            return result.Content;
        }
        #endregion

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

            #region Creating Driver for DMS request
            //TODO: Get PIC Detail for Driver
            UserRequest dmsRequest = new UserRequest()
            {
                Requests = new List<User>()
                        {
                            new User()
                            {
                                DriverNo = driverRequest.Requests[0].DriverNo,
                                UserName = driverRequest.Requests[0].UserName,
                                Password = driverRequest.Requests[0].Password,
                                FirstName = driverRequest.Requests[0].FirstName,
                                LastName = driverRequest.Requests[0].LastName,
                                Email = driverRequest.Requests[0].Email,
                                PhoneNumber = driverRequest.Requests[0].DriverPhone,
                                PICName = "",
                                PICEmail = "",
                                PICPhone = "",
                                IsActive = true,
                                CreatedBy = "SYSTEM"
                            }
                        },
                CreatedBy = "SYSTEM",
                LastModifiedBy = driverRequest.LastModifiedBy,
                CreatedTime = driverRequest.CreatedTime,
                LastModifiedTime = driverRequest.LastModifiedTime
            };
            #endregion

            IDriverTask driverTask = DependencyResolver.GetImplementationOf<ITaskGateway>().DriverTask;
            DriverResponse driverResponse = driverTask.CreateUpdateDriver(driverRequest);

            if (driverResponse != null && driverResponse.StatusCode == (int)HttpStatusCode.OK && driverResponse.StatusMessage == DomainObjects.Resource.ResourceData.DriversCreated)
            {
                #region Create Driver in DMS
                dmsRequest.Requests[0].DriverNo = driverResponse.Data[0].DriverNo;
                LoginRequest loginRequest = new LoginRequest();
                string token = "";

                //Login to DMS and get Token
                loginRequest.UserName = ConfigurationManager.AppSettings["DMSLogin"];
                loginRequest.UserPassword = ConfigurationManager.AppSettings["DMSPassword"];
                var dmsLoginResponse = JsonConvert.DeserializeObject<UserResponse>(GetApiResponse(ConfigurationManager.AppSettings["ApiGatewayDMSURL"]
                    + "/v1/user/login", Method.POST, loginRequest, null));
                if (dmsLoginResponse != null && dmsLoginResponse.Data.Count > 0)
                {
                    token = dmsLoginResponse.TokenKey;
                }

                var userResponse = JsonConvert.DeserializeObject<UserResponse>(GetApiResponse(ConfigurationManager.AppSettings["ApiGatewayDMSURL"]
                    + "/v1/user/createupdateuser", Method.POST, dmsRequest, token));
                if (userResponse != null)
                {
                    driverResponse.StatusMessage = driverResponse.StatusMessage + ". " + userResponse.StatusMessage;
                }

                #endregion
            }

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
