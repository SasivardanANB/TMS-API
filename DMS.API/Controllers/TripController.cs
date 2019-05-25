﻿using DMS.API.Classes;
using DMS.DomainGateway.Gateway.Interfaces;
using DMS.DomainGateway.Task.Interfaces;
using DMS.DomainObjects.Objects;
using DMS.DomainObjects.Request;
using DMS.DomainObjects.Response;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace DMS.API.Controllers
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

        private string GetOrderNumber(int stopPointId)
        {
            ITripTask tripTask = Helper.Model.DependencyResolver.DependencyResolver.GetImplementationOf<ITaskGateway>().TripTask;
            return tripTask.GetOrderNumber(stopPointId);
        }

        private string GetOrderStatusCode(int tripStatusId)
        {
            ITripTask tripTask = Helper.Model.DependencyResolver.DependencyResolver.GetImplementationOf<ITaskGateway>().TripTask;
            return tripTask.GetOrderStatusCode(tripStatusId);
        }

        private int GetOrderSequnceNumber(int stopPointId)
        {
            ITripTask tripTask = Helper.Model.DependencyResolver.DependencyResolver.GetImplementationOf<ITaskGateway>().TripTask;
            return tripTask.GetOrderSequnceNumber(stopPointId);
        }
        #endregion

        [Route("createupdatetrip")]
        [HttpPost]
        public IHttpActionResult CreateUpdateTrip(TripRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            ITripTask tripTask = Helper.Model.DependencyResolver.DependencyResolver.GetImplementationOf<ITaskGateway>().TripTask;
            TripResponse tripData = tripTask.CreateUpdateTrip(request);
            return Ok(tripData);
        }

        [Route("gettrips")]
        [HttpPost]
        public IHttpActionResult GetTripsByDriver(TripsByDriverRequest tripsByDriverRequest)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            ITripTask tripTask = Helper.Model.DependencyResolver.DependencyResolver.GetImplementationOf<ITaskGateway>().TripTask;
            TripResponse tripData = tripTask.GetTripsByDriver(tripsByDriverRequest);
            return Ok(tripData);
        }

        [Route("getstoppoints")]
        [HttpPost]
        public IHttpActionResult GetStopPointsByTrip(StopPointsRequest stopPointsByTripRequest)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            ITripTask tripTask = Helper.Model.DependencyResolver.DependencyResolver.GetImplementationOf<ITaskGateway>().TripTask;
            StopPointsResponse tripData = tripTask.GetStopPointsByTrip(stopPointsByTripRequest);
            return Ok(tripData);
        }

        [Route("stoppoint/getorderitems")]
        [HttpPost]
        public IHttpActionResult GetOrderItemsByStopPoint(StopPointsRequest stopPointOrderItemsRequest)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            ITripTask tripTask = Helper.Model.DependencyResolver.DependencyResolver.GetImplementationOf<ITaskGateway>().TripTask;
            StopPointOrderItemsResponse tripData = tripTask.GetOrderItemsByStopPoint(stopPointOrderItemsRequest);
            return Ok(tripData);
        }

        [Route("updatestatus")]
        [HttpPost]
        public IHttpActionResult UpdateTripStatusEventLog(UpdateTripStatusRequest updateTripStatusRequest)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            ITripTask tripTask = Helper.Model.DependencyResolver.DependencyResolver.GetImplementationOf<ITaskGateway>().TripTask;
            UpdateTripStatusResponse tripData = tripTask.UpdateTripStatusEventLog(updateTripStatusRequest);
            

            #region Update Status to TMS 
            OrderStatusRequest tmsRequest = new OrderStatusRequest()
            {
                Requests = new List<OrderStatus>()
            };

            foreach (var item in updateTripStatusRequest.Requests)
            {
                #region Get Order Number by Stop Point
                string orderNumber = GetOrderNumber(item.StopPointId);
                #endregion

                #region Get Trip Status Code by Status ID
                string orderStatusCode = GetOrderStatusCode(item.TripStatusId);
                #endregion

                #region Get Trip Sequnce Number
                int orderSequenceNumber = GetOrderSequnceNumber(item.StopPointId);
                #endregion

                OrderStatus requestData = new OrderStatus()
                {
                    IsLoad = item.IsLoad ? 1 : 0,
                    OrderNumber = orderNumber,
                    OrderStausCode = orderStatusCode,
                    Remarks = item.Remarks,
                    SequenceNumber = orderSequenceNumber
                };

                tmsRequest.Requests.Add(requestData);
            }

            if (tmsRequest.Requests.Count > 0)
            {
                #region Call TMS API to Update Order
                #region Login to TMS and get Token
                LoginRequest loginRequest = new LoginRequest();
                string token = "";

                loginRequest.UserName = ConfigurationManager.AppSettings["TMSLogin"];
                loginRequest.UserPassword = ConfigurationManager.AppSettings["TMSPassword"];
                var tmsLoginResponse = JsonConvert.DeserializeObject<UserResponse>(GetApiResponse(ConfigurationManager.AppSettings["ApiGatewayTMSURL"]
                    + "/v1/user/login", Method.POST, loginRequest, null));
                if (tmsLoginResponse != null && tmsLoginResponse.Data.Count > 0)
                {
                    token = tmsLoginResponse.TokenKey;
                }
                #endregion

                var response = JsonConvert.DeserializeObject<UserResponse>(GetApiResponse(ConfigurationManager.AppSettings["ApiGatewayTMSURL"]
                    + "/v1/order/updateorderstatus", Method.POST, tmsRequest, token));
                if (response != null)
                {
                    tripData.StatusMessage += ". " + response.StatusMessage;
                }
                #endregion
            }
            #endregion

            return Ok(tripData);
        }

        

        [Route("updatetripstatus")]
        [HttpPost]
        public IHttpActionResult UpdateEntireTripStatus(TripsByDriverRequest tripsByDriverRequest)
        {
            for (int i = 0; i < tripsByDriverRequest.Requests.Count; i++)
            {
                ModelState.Remove("tripsByDriverRequest.Requests[" + i + "]");
                ModelState.Remove("tripsByDriverRequest.Requests[" + i + "].OrderType");
            }
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            ITripTask tripTask = Helper.Model.DependencyResolver.DependencyResolver.GetImplementationOf<ITaskGateway>().TripTask;
            TripResponse tripData = tripTask.UpdateEntireTripStatus(tripsByDriverRequest);
            return Ok(tripData);
        }
    }
}
