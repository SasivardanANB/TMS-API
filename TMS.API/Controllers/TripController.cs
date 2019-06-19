using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TMS.API.Classes;
using TMS.DomainGateway.Gateway.Interfaces;
using TMS.DomainGateway.Task.Interfaces;
using TMS.DomainObjects.Objects;
using TMS.DomainObjects.Request;
using TMS.DomainObjects.Response;


namespace TMS.API.Controllers
{


    [CustomAuthorize]
    [RoutePrefix("api/v1/trip")]
    public class TripController : ApiController
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


        [Route("gettriplist")]
        [HttpPost]
        public IHttpActionResult GetTripList(TripRequest tripRequest)
        {
            ITripTask tripTask = Helper.Model.DependencyResolver.DependencyResolver.GetImplementationOf<ITaskGateway>().TripTask;
            TripResponse tripResponse = tripTask.GetTripList(tripRequest);
            return Ok(tripResponse);
        }

        [Route("gettripdetails")]
        [HttpGet]
        public IHttpActionResult GetTripDetails(int orderId)
        {
            ITripTask tripTask = Helper.Model.DependencyResolver.DependencyResolver.GetImplementationOf<ITaskGateway>().TripTask;
            OrderDetailsResponse orderDetailsResponse = tripTask.GetTripDetails(orderId);
            return Ok(orderDetailsResponse);
        }

        [Route("updatetripdetails")]
        [HttpPost]
        public IHttpActionResult UpdateTripDetails(TripRequest tripRequest)
        {
            ITripTask tripTask = Helper.Model.DependencyResolver.DependencyResolver.GetImplementationOf<ITaskGateway>().TripTask;
            TripResponse tripResponse = tripTask.UpdateTripDetails(tripRequest);

            if (tripResponse.StatusCode == 200 && tripResponse.Status == "Success" && tripResponse.Data.Count >0)
            {
                #region Creating trip object to Update Trip Details
                TripRequest tripRequest1 = new TripRequest();
                List<Trip> trips = new List<Trip>();
                foreach (var response in tripResponse.Data)
                {
                    Trip trip = new Trip();
                    trip.OrderNumber = response.OrderNumber;
                    trip.VehicleType = response.VehicleType;
                    trip.DriverNo = response.DriverNo;
                    trip.DriverName = response.DriverName;
                    trips.Add(trip);
                }
                tripRequest1.Requests = trips;
                tripRequest1.LastModifiedTime = DateTime.Now;
                tripRequest1.LastModifiedBy = tripRequest.LastModifiedBy;
                if(tripRequest1.Requests.Count > 0)
                {
                    #region Calling DMS API to updatedriver details in trip
                    #region Login to DMS and get Token
                    LoginRequest loginRequest = new LoginRequest();
                    string token = "";
                    loginRequest.UserName = ConfigurationManager.AppSettings["DMSLogin"];
                    loginRequest.UserPassword = ConfigurationManager.AppSettings["DMSPassword"];
                    var dmsLoginResponse = JsonConvert.DeserializeObject<UserResponse>(GetApiResponse(ConfigurationManager.AppSettings["ApiGatewayDMSURL"]
                        + "/v1/user/login", Method.POST, loginRequest, null));
                    if (dmsLoginResponse != null && dmsLoginResponse.Data.Count > 0)
                    {
                        token = dmsLoginResponse.TokenKey;
                    }
                    #endregion
                    #region Call DMS API to assign order to driver
                    var response = JsonConvert.DeserializeObject<TripResponse>(GetApiResponse(ConfigurationManager.AppSettings["ApiGatewayDMSURL"]
                        + "/v1/trip/reassigntrip", Method.POST, tripRequest1, token));
                    if (response != null)
                    {
                        tripResponse.StatusMessage += ". " + response.StatusMessage;
                    }
                    #endregion

                    #endregion

                }
                #endregion
            }

            return Ok(tripResponse);
        }
    }
}
