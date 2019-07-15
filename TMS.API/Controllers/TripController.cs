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
            IEnumerable<string> headerValues;
            var tokenValue = string.Empty;
            if (Request.Headers.TryGetValues("Token", out headerValues))
            {
                tokenValue = headerValues.FirstOrDefault();
                tripRequest.Token = tokenValue;
            }
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
            // RE-ASSIGN TRIP IN TMS
            ITripTask tripTask = Helper.Model.DependencyResolver.DependencyResolver.GetImplementationOf<ITaskGateway>().TripTask;
            TripResponse tripResponse = tripTask.UpdateTripDetails(tripRequest);

            if (tripResponse.StatusCode == (int)HttpStatusCode.OK && tripResponse.Status == DomainObjects.Resource.ResourceData.Success && tripResponse.Data.Count > 0)
            {
                // Creating trip object to Update Trip Details
                TripRequestDMS tripRequest1 = new TripRequestDMS();
                List<TripDMS> trips = new List<TripDMS>();
                foreach (var response in tripResponse.Data)
                {
                    TripDMS trip = new TripDMS();
                    trip.OrderNumber = response.OrderNumber;
                    trip.VehicleType = response.VehicleType;
                    trip.VehicleNumber = response.Vehicle;
                    trip.DriverNo = response.DriverNo;
                    trip.DriverName = response.DriverName;
                    trips.Add(trip);
                }
                tripRequest1.Requests = trips;
                tripRequest1.LastModifiedTime = DateTime.Now;
                tripRequest1.LastModifiedBy = tripRequest.LastModifiedBy;

                if (tripRequest1.Requests.Count > 0)
                {
                    #region Update Trip in DMS
                    // Login to DMS and get Token
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

                    // Call DMS API to assign order to driver
                    var response = JsonConvert.DeserializeObject<TripResponse>(GetApiResponse(ConfigurationManager.AppSettings["ApiGatewayDMSURL"]
                        + "/v1/trip/reassigntrip", Method.POST, tripRequest1, token));
                    #endregion

                    if (response != null && response.StatusCode == (int)HttpStatusCode.OK && response.Status == DomainObjects.Resource.ResourceData.Success)
                    {
                        tripResponse.StatusMessage += " " + response.StatusMessage;

                        // Creating trip object to Update Trip Details
                        TripRequest omsTripRequest = new TripRequest();
                        List<Trip> omsTrips = new List<Trip>();
                        foreach (var tripresponse in tripResponse.Data)
                        {
                            Trip trip = new Trip();
                            trip.OrderNumber = tripresponse.OrderNumber;
                            trip.VehicleType = tripresponse.VehicleType;
                            trip.Vehicle = tripresponse.Vehicle;
                            trip.DriverNo = tripresponse.DriverNo;
                            trip.DriverName = tripresponse.DriverName;
                            omsTrips.Add(trip);
                        }
                        omsTripRequest.Requests = omsTrips;
                        omsTripRequest.LastModifiedTime = DateTime.Now;
                        omsTripRequest.LastModifiedBy = tripRequest.LastModifiedBy;
                        if (omsTripRequest.Requests.Count > 0)
                        {
                            #region Update Trip in OMS
                            //Login to DMS and get Token
                            LoginRequest loginRequestOMS = new LoginRequest();
                            string tokenOMS = "";
                            loginRequest.UserName = ConfigurationManager.AppSettings["OMSLogin"];
                            loginRequest.UserPassword = ConfigurationManager.AppSettings["OMSPassword"];
                            var omsLoginResponse = JsonConvert.DeserializeObject<UserResponse>(GetApiResponse(ConfigurationManager.AppSettings["ApiGatewayOMSURL"]
                                + "/v1/user/login", Method.POST, loginRequest, null));
                            if (omsLoginResponse != null && omsLoginResponse.Data.Count > 0)
                            {
                                tokenOMS = omsLoginResponse.TokenKey;
                            }
                            //Call OMS API to assign order to driver
                            var omsResponse = JsonConvert.DeserializeObject<TripResponse>(GetApiResponse(ConfigurationManager.AppSettings["ApiGatewayOMSURL"]
                                + "/v1/order/reassigntrip", Method.POST, omsTripRequest, tokenOMS));
                            if (omsResponse != null)
                            {
                                tripResponse.StatusMessage = omsResponse.StatusMessage + " " + tripResponse.StatusMessage;
                            }
                            #endregion
                        }
                    }
                }
            }

            return Ok(tripResponse);
        }
    }
}
