using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using TMS.BusinessGateway.Classes;
using TMS.DataGateway.Repositories.Interfaces;
using TMS.DataGateway.Repositories.Iterfaces;
using TMS.DomainGateway.Task;
using TMS.DomainObjects.Objects;
using TMS.DomainObjects.Request;
using TMS.DomainObjects.Response;

namespace TMS.BusinessGateway.Task
{
    public partial class BusinessTripTask : TripTask
    {
        private readonly ITrip _tripRepository;

        public BusinessTripTask(ITrip tripRepository)
        {
            _tripRepository = tripRepository;
        }

        public override TripResponse GetTripList(TripRequest tripRequest)
        {
            TripResponse tripResponse = _tripRepository.GetTripList(tripRequest);
            return tripResponse;
        }

        public override OrderDetailsResponse GetTripDetails(int orderId)
        {
            OrderDetailsResponse orderDetailsResponse = _tripRepository.GetTripDetails(orderId);
            return orderDetailsResponse;
        }

        public override TripResponse UpdateTripDetails(TripRequest tripRequest)
        {
            TripResponse tripResponse = _tripRepository.UpdateTripDetails(tripRequest);

            if (tripResponse.StatusCode == (int)HttpStatusCode.OK && tripResponse.Status == DomainObjects.Resource.ResourceData.Success && tripResponse.Data.Count > 0)
            {
                // Creating trip object to Update Trip Details
                TripRequestDMS tripRequest1 = new TripRequestDMS();
                List<TripDMS> trips = new List<TripDMS>();
                foreach (var response in tripResponse.Data)
                {
                    TripDMS trip = new TripDMS
                    {
                        OrderNumber = response.OrderNumber,
                        VehicleType = response.VehicleType,
                        VehicleNumber = response.Vehicle,
                        DriverNo = response.DriverNo,
                        DriverName = response.DriverName
                    };
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
                    string token = string.Empty;
                    loginRequest.UserName = ConfigurationManager.AppSettings["DMSLogin"];
                    loginRequest.UserPassword = ConfigurationManager.AppSettings["DMSPassword"];
                    var dmsLoginResponse = JsonConvert.DeserializeObject<UserResponse>(Utility.GetApiResponse(ConfigurationManager.AppSettings["ApiGatewayDMSURL"]
                        + "/v1/user/login", Method.POST, loginRequest, null));

                    if (dmsLoginResponse != null && dmsLoginResponse.Data.Count > 0)
                    {
                        token = dmsLoginResponse.TokenKey;
                    }

                    // Call DMS API to assign order to driver
                    var response = JsonConvert.DeserializeObject<TripResponse>(Utility.GetApiResponse(ConfigurationManager.AppSettings["ApiGatewayDMSURL"]
                        + "/v1/trip/reassigntrip", Method.POST, tripRequest1, token));

                    #endregion

                    if (response != null && response.StatusCode == (int)HttpStatusCode.OK && response.Status == DomainObjects.Resource.ResourceData.Success)
                    {
                        tripResponse.StatusMessage += " " + response.StatusMessage;

                        // Creating trip object to Update Trip Details in OMS
                        TripRequest omsTripRequest = new TripRequest();
                        List<Trip> omsTrips = new List<Trip>();
                        foreach (var tripresponse in tripResponse.Data)
                        {
                            Trip trip = new Trip
                            {
                                OrderNumber = tripresponse.OrderNumber,
                                VehicleType = tripresponse.VehicleType,
                                Vehicle = tripresponse.Vehicle,
                                DriverNo = tripresponse.DriverNo,
                                DriverName = tripresponse.DriverName
                            };
                            omsTrips.Add(trip);
                        }
                        omsTripRequest.Requests = omsTrips;
                        omsTripRequest.LastModifiedTime = DateTime.Now;
                        omsTripRequest.LastModifiedBy = tripRequest.LastModifiedBy;

                        if (omsTripRequest.Requests.Count > 0)
                        {
                            #region Update Trip in OMS

                            //Login to OMS and get Token
                            string tokenOMS = string.Empty;
                            loginRequest.UserName = ConfigurationManager.AppSettings["OMSLogin"];
                            loginRequest.UserPassword = ConfigurationManager.AppSettings["OMSPassword"];
                            var omsLoginResponse = JsonConvert.DeserializeObject<UserResponse>(Utility.GetApiResponse(ConfigurationManager.AppSettings["ApiGatewayOMSURL"]
                                + "/v1/user/login", Method.POST, loginRequest, null));

                            if (omsLoginResponse != null && omsLoginResponse.Data.Count > 0)
                            {
                                tokenOMS = omsLoginResponse.TokenKey;
                            }

                            //Call OMS API to assign order to driver
                            var omsResponse = JsonConvert.DeserializeObject<TripResponse>(Utility.GetApiResponse(ConfigurationManager.AppSettings["ApiGatewayOMSURL"]
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

            return tripResponse;
        }
    }
}
