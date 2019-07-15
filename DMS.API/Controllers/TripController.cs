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

            if (tripData.StatusCode == 200 && tripData.Status == "Success" && request.Requests.Count > 0 && ConfigurationManager.AppSettings["AllowPushNotifications"].ToString() == "true")
            {
                int i = 0;
                foreach (var reqObj in request.Requests)
                {
                    string deviceId = tripTask.GetDeviceId(reqObj.DriverNo);
                    if (!string.IsNullOrEmpty(deviceId))
                    {
                        var client = new RestClient("https://fcm.googleapis.com/");
                        client.AddDefaultHeader("Content-Type", "application/json");
                        client.AddDefaultHeader("Authorization", ConfigurationManager.AppSettings["FCM_Authorization"]);
                        var req = new RestRequest("fcm/send", Method.POST) { RequestFormat = DataFormat.Json };
                        req.Timeout = 500000;
                        NotificationRequest notificationRequest = new NotificationRequest();
                        notificationRequest.to = deviceId;   //"cFaYBS5Rn_w:APA91bEcu9Q_wqSASgXZ1nAUUvpelCTOw6eF5g0RmdNIrIi7GlJl-mTezk9Tb7lVkzzBH3wabznQ6GMkws2Br9XV8OvpilwSmMjcE3MKe9LrEtYZ8eAodgbx12-Az0NU6_IKbIfB0POu";

                        string tripNumber = "";
                        if (tripData.Data != null)
                        {
                            tripNumber = tripData.Data[i].TripNumber;
                        }

                        notificationRequest.data = new Notification()
                        {
                            title = "Trip has been assigned to you.",

                            message = " " + tripNumber,
                            click_action = "NOTIFICATIONACTIVITY",
                        };

                        req.AddJsonBody(notificationRequest);
                        var result = client.Execute(req);
                        i++;
                    }
                }

            }
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
                    IsLoad = item.IsLoad,
                    OrderNumber = orderNumber,
                    OrderStatusCode = orderStatusCode,
                    Remarks = item.Remarks,
                    SequenceNumber = item.SequenceNumber,
                    NewSequenceNumber = orderSequenceNumber
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

            #region Update Status to TMS 
            OrderStatusRequest tmsRequest = new OrderStatusRequest()
            {
                Requests = new List<OrderStatus>()
            };

            foreach (var item in tripsByDriverRequest.Requests)
            {
                #region Get Order Number by Stop Point
                int stoppointId = tripData.Data.FirstOrDefault(t => t.ID == item.ID).StopPoints.FirstOrDefault().ID;
                string orderNumber = GetOrderNumber(stoppointId);
                #endregion

                #region Get Trip Status Code by Status ID
                string orderStatusCode = GetOrderStatusCode(item.TripStatusId.Value);
                #endregion

                #region Get Trip Sequnce Number
                int orderSequenceNumber = 0;
                #endregion

                OrderStatus requestData = new OrderStatus()
                {
                    IsLoad = null,
                    OrderNumber = orderNumber,
                    OrderStatusCode = orderStatusCode,
                    Remarks = "",
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

            if (tripData.StatusCode == 200 && tripData.Status == "Success" && tripRequest.Requests.Count > 0 && ConfigurationManager.AppSettings["AllowPushNotifications"].ToString() == "true")
            {
                int i = 0;
                foreach (var reqObj in tripRequest.Requests)
                {
                    string deviceId = tripTask.GetDeviceId(reqObj.DriverNo);
                    if (!string.IsNullOrEmpty(deviceId))
                    {
                        var client = new RestClient("https://fcm.googleapis.com/");
                        client.AddDefaultHeader("Content-Type", "application/json");
                        client.AddDefaultHeader("Authorization", ConfigurationManager.AppSettings["FCM_Authorization"]);
                        var req = new RestRequest("fcm/send", Method.POST) { RequestFormat = DataFormat.Json };
                        req.Timeout = 500000;
                        NotificationRequest notificationRequest = new NotificationRequest();
                        notificationRequest.to = deviceId;   //"cFaYBS5Rn_w:APA91bEcu9Q_wqSASgXZ1nAUUvpelCTOw6eF5g0RmdNIrIi7GlJl-mTezk9Tb7lVkzzBH3wabznQ6GMkws2Br9XV8OvpilwSmMjcE3MKe9LrEtYZ8eAodgbx12-Az0NU6_IKbIfB0POu";

                        string tripNumber = "";
                        if (tripData.Data != null)
                        {
                            tripNumber = tripData.Data[i].TripNumber;
                        }

                        notificationRequest.data = new Notification()
                        {
                            title = "Trip has been assigned to you.",

                            message = " " + tripNumber,
                            click_action = "NOTIFICATIONACTIVITY",
                        };

                        req.AddJsonBody(notificationRequest);
                        var result = client.Execute(req);
                        i++;
                    }
                }

            }

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

            ShipmentListRequest shipmentListRequest = new ShipmentListRequest()
            {
                Requests = new List<ShipmentListDetails>()
            };

            foreach (string packingSheetNo in shippingList.PKG_List)
            {
                ShipmentListDetails shipmentListDetails = new ShipmentListDetails();
                int.TryParse(shippingList.Colie.Trim(), out int numberOfBoxes);
                shipmentListDetails.NumberOfBoxes = numberOfBoxes;
                shipmentListDetails.Note = "Pack";
                shipmentListDetails.PackingSheetNumber = packingSheetNo;
                shipmentListDetails.StopPointId = stopPointId;

                shipmentListRequest.Requests.Add(shipmentListDetails);
            }

            ITripTask tripTask = Helper.Model.DependencyResolver.DependencyResolver.GetImplementationOf<ITaskGateway>().TripTask;
            ShipmentListResponse shipmentListResponse = tripTask.CreateUpdateShipmentList(shipmentListRequest);

            return Ok(shipmentListResponse);
        }

        private class ShippingList
        {
            public string Colie { get; set; } = string.Empty;
            public List<String> PKG_List { get; set; } = null;
            public string SL_No { get; set; } = string.Empty;
        }
    }
}
