using DMS.API.Classes;
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
using System.Threading.Tasks;
using System.Web.Http;

namespace DMS.API.Controllers
{
    [CustomAuthorize]
    [RoutePrefix("api/v1/trip")]
    public class TripController : ApiController
    {
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

        [Route("getlasttripstatus")]
        [HttpGet]
        public IHttpActionResult GetLastTripStatusData(int stopPointId)
        {
            ITripTask tripTask = Helper.Model.DependencyResolver.DependencyResolver.GetImplementationOf<ITaskGateway>().TripTask;
            StopPointsResponse tripData = tripTask.GetLastTripStatusData(stopPointId);
            return Ok(tripData);
        }

        [Route("reassigntrip")]
        [HttpPost]
        public IHttpActionResult ReAssignTrip(TripRequest tripRequest)
        {
            ITripTask tripTask = Helper.Model.DependencyResolver.DependencyResolver.GetImplementationOf<ITaskGateway>().TripTask;
            TripResponse tripData = tripTask.ReAssignTrip(tripRequest);

            return Ok(tripData);
        }

        [Route("getshippinglistguids")]
        [HttpGet]
        public IHttpActionResult GetShippingListGuids(string orderNumber)
        {
            ITripTask tripTask = Helper.Model.DependencyResolver.DependencyResolver.GetImplementationOf<ITaskGateway>().TripTask;
            ImageGuidsResponse imageGuidsResponse = tripTask.GetShippingListGuids(orderNumber);
            return Ok(imageGuidsResponse);
        }

        [Route("getpodguids")]
        [HttpGet]
        public IHttpActionResult GetPodGuids(string orderNumber)
        {
            ITripTask tripTask = Helper.Model.DependencyResolver.DependencyResolver.GetImplementationOf<ITaskGateway>().TripTask;
            ImageGuidsResponse imageGuidsResponse = tripTask.GetPodGuids(orderNumber);
            return Ok(imageGuidsResponse);
        }

        [Route("getphotowithcustomerguids")]
        [HttpGet]
        public IHttpActionResult GetPhotoWithCustomerGuids(string orderNumber)
        {
            ITripTask tripTask = Helper.Model.DependencyResolver.DependencyResolver.GetImplementationOf<ITaskGateway>().TripTask;
            ImageGuidsResponse imageGuidsResponse = tripTask.GetPhotoWithCustomerGuids(orderNumber);
            return Ok(imageGuidsResponse);
        }

        [Route("getpendingstoppoints")]
        [HttpGet]
        public IHttpActionResult GetPendingStopPoints(int tripId)
        {
            ITripTask tripTask = Helper.Model.DependencyResolver.DependencyResolver.GetImplementationOf<ITaskGateway>().TripTask;
            StopPointsResponse stopPointsResponse = tripTask.GetPendingStopPoints(tripId);
            return Ok(stopPointsResponse);
        }

        [Route("cancelorder")]
        [HttpPost]
        public IHttpActionResult CancelOrder(OrderStatusRequest request)
        {
            ITripTask tripTask = Helper.Model.DependencyResolver.DependencyResolver.GetImplementationOf<ITaskGateway>().TripTask;
            OrderStatusResponse response = tripTask.CancelOrder(request);
            return Ok(response);
        }

        [HttpPost, Route("shippinglistocr")]
        public async Task<IHttpActionResult> ShippingListOCR(int stopPointId)
        {
            if (!Request.Content.IsMimeMultipartContent("form-data"))
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }

            var client = new HttpClient();
            var response = await client.PostAsync(ConfigurationManager.AppSettings["ShipmentListOCRURL"], Request.Content);
            var json = response.Content.ReadAsStringAsync().Result;

            ShippingList shippingList = JsonConvert.DeserializeObject<ShippingList>(json);

            ITripTask tripTask = Helper.Model.DependencyResolver.DependencyResolver.GetImplementationOf<ITaskGateway>().TripTask;
            ShipmentListResponse shipmentListResponse = tripTask.CreateUpdateShipmentList(stopPointId, shippingList);

            return Ok(shipmentListResponse);
        }

        [Route("swapestoppoints")]
        [HttpPost]
        public IHttpActionResult SwapeStopPoints(UpdateTripStatusRequest updateTripStatusRequest)
        {
            ITripTask tripTask = Helper.Model.DependencyResolver.DependencyResolver.GetImplementationOf<ITaskGateway>().TripTask;
            StopPointsResponse stopPointsResponse = tripTask.SwapeStopPoints(updateTripStatusRequest);

            return Ok(stopPointsResponse);
        }
    }
}
