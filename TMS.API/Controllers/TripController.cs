﻿using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
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
            // Re-assign trip in TMS
            ITripTask tripTask = Helper.Model.DependencyResolver.DependencyResolver.GetImplementationOf<ITaskGateway>().TripTask;
            TripResponse tripResponse = tripTask.UpdateTripDetails(tripRequest);

            return Ok(tripResponse);
        }
    }
}
