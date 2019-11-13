using DMS.BusinessGateway.Classes;
using DMS.DataGateway.Repositories.Iterfaces;
using DMS.DomainGateway.Task;
using DMS.DomainObjects.Objects;
using DMS.DomainObjects.Request;
using DMS.DomainObjects.Resource;
using DMS.DomainObjects.Response;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;

namespace DMS.BusinessGateway.Task
{
    public partial class BusinessTripTask : TripTask
    {
        private readonly ITrip _tripRepository;

        public BusinessTripTask(ITrip tripRepository)
        {
            _tripRepository = tripRepository;
        }

        public override StopPointOrderItemsResponse GetOrderItemsByStopPoint(StopPointsRequest stopPointsByTripRequest)
        {
            StopPointOrderItemsResponse tripData = _tripRepository.GetOrderItemsByStopPoint(stopPointsByTripRequest);
            return tripData;
        }

        public override StopPointsResponse GetStopPointsByTrip(StopPointsRequest stopPointsByTripRequest)
        {
            StopPointsResponse tripData = _tripRepository.GetStopPointsByTrip(stopPointsByTripRequest);
            return tripData;
        }

        public override TripResponse GetTripsByDriver(TripsByDriverRequest tripsByDriverRequest)
        {
            TripResponse tripData = _tripRepository.GetTripsByDriver(tripsByDriverRequest);
            return tripData;
        }

        public override UpdateTripStatusResponse UpdateTripStatusEventLog(UpdateTripStatusRequest updateTripStatusRequest)
        {
            UpdateTripStatusResponse tripData = _tripRepository.UpdateTripStatusEventLog(updateTripStatusRequest);

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
                var tmsLoginResponse = JsonConvert.DeserializeObject<UserResponse>(Utility.GetApiResponse(ConfigurationManager.AppSettings["ApiGatewayTMSURL"]
                    + "/v1/user/login", Method.POST, loginRequest, null));
                if (tmsLoginResponse != null && tmsLoginResponse.Data.Count > 0)
                {
                    token = tmsLoginResponse.TokenKey;
                }
                #endregion

                var response = JsonConvert.DeserializeObject<UserResponse>(Utility.GetApiResponse(ConfigurationManager.AppSettings["ApiGatewayTMSURL"]
                    + "/v1/order/updateorderstatus", Method.POST, tmsRequest, token));
                if (response != null)
                {
                    tripData.StatusMessage += ". " + response.StatusMessage;
                }
                #endregion
            }
            #endregion

            return tripData;
        }

        public override TripResponse UpdateEntireTripStatus(TripsByDriverRequest tripsByDriverRequest)
        {
            TripResponse tripData = _tripRepository.UpdateEntireTripStatus(tripsByDriverRequest);

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

                OrderStatus requestData = new OrderStatus()
                {
                    IsLoad = null,
                    OrderNumber = orderNumber,
                    OrderStatusCode = orderStatusCode,
                    Remarks = "",
                    SequenceNumber = 0,
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
                var tmsLoginResponse = JsonConvert.DeserializeObject<UserResponse>(Utility.GetApiResponse(ConfigurationManager.AppSettings["ApiGatewayTMSURL"]
                    + "/v1/user/login", Method.POST, loginRequest, null));
                if (tmsLoginResponse != null && tmsLoginResponse.Data.Count > 0)
                {
                    token = tmsLoginResponse.TokenKey;
                }
                #endregion

                var response = JsonConvert.DeserializeObject<UserResponse>(Utility.GetApiResponse(ConfigurationManager.AppSettings["ApiGatewayTMSURL"]
                    + "/v1/order/updateorderstatus", Method.POST, tmsRequest, token));
                if (response != null)
                {
                    tripData.StatusMessage += ". " + response.StatusMessage;
                }
                #endregion
            }
            #endregion

            return tripData;
        }

        public override TripResponse CreateUpdateTrip(TripRequest request)
        {
            TripResponse tripData = _tripRepository.CreateUpdateTrip(request);

            if (tripData.StatusCode == (int)HttpStatusCode.OK && tripData.Status == ResourceData.Success && request.Requests.Count > 0 && Convert.ToBoolean(ConfigurationManager.AppSettings["AllowPushNotifications"]))
            {
                int i = 0;
                foreach (var reqObj in request.Requests)
                {
                    string deviceId = GetDeviceId(reqObj.DriverNo);
                    if (!string.IsNullOrEmpty(deviceId))
                    {
                        var client = new RestClient(ConfigurationManager.AppSettings["FCMURL"]);
                        client.AddDefaultHeader("Content-Type", "application/json");
                        client.AddDefaultHeader("Authorization", ConfigurationManager.AppSettings["FCM_Authorization"]);
                        var req = new RestRequest("fcm/send", Method.POST) { RequestFormat = DataFormat.Json };
                        req.Timeout = 500000;
                        NotificationRequest notificationRequest = new NotificationRequest();
                        notificationRequest.to = deviceId;   

                        string tripNumber = string.Empty;
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
                        client.Execute(req);
                        i++;
                    }
                }
            }

            return tripData;
        }

        public override string GetOrderNumber(int stopPointId)
        {
            return _tripRepository.GetOrderNumber(stopPointId);
        }

        public override string GetOrderStatusCode(int tripStatusId)
        {
            return _tripRepository.GetOrderStatusCode(tripStatusId);
        }

        public override int GetOrderSequnceNumber(int stopPointId)
        {
            return _tripRepository.GetOrderSequnceNumber(stopPointId);
        }

        public override StopPointsResponse GetLastTripStatusData(int stopPointId)
        {
            return _tripRepository.GetLastTripStatusData(stopPointId);
        }

        public override string GetDeviceId(string token)
        {
            return _tripRepository.GetDeviceId(token);
        }

        public override TripResponse ReAssignTrip(TripRequest tripRequest)
        {
            TripResponse tripData = _tripRepository.ReAssignTrip(tripRequest);

            if (tripData.StatusCode == (int)HttpStatusCode.OK && tripData.Status == ResourceData.Success && tripRequest.Requests.Count > 0 && Convert.ToBoolean(ConfigurationManager.AppSettings["AllowPushNotifications"]))
            {
                int i = 0;
                foreach (var reqObj in tripRequest.Requests)
                {
                    string deviceId = GetDeviceId(reqObj.DriverNo);
                    if (!string.IsNullOrEmpty(deviceId))
                    {
                        var client = new RestClient(ConfigurationManager.AppSettings["FCMURL"]);
                        client.AddDefaultHeader("Content-Type", "application/json");
                        client.AddDefaultHeader("Authorization", ConfigurationManager.AppSettings["FCM_Authorization"]);
                        var req = new RestRequest("fcm/send", Method.POST) { RequestFormat = DataFormat.Json };
                        req.Timeout = 500000;
                        NotificationRequest notificationRequest = new NotificationRequest();
                        notificationRequest.to = deviceId;   

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
                        client.Execute(req);
                        i++;
                    }
                }
            }

            return tripData;
        }

        public override ImageGuidsResponse GetShippingListGuids(string orderNumber)
        {
            return _tripRepository.GetShippingListGuids(orderNumber);
        }

        public override ImageGuidsResponse GetPodGuids(string orderNumber)
        {
            return _tripRepository.GetPodGuids(orderNumber);
        }

        public override ImageGuidsResponse GetPhotoWithCustomerGuids(string orderNumber)
        {
            return _tripRepository.GetPhotoWithCustomerGuids(orderNumber);
        }

        public override StopPointsResponse GetPendingStopPoints(int tripId)
        {
            return _tripRepository.GetPendingStopPoints(tripId);
        }

        public override OrderStatusResponse CancelOrder(OrderStatusRequest request)
        {
            return _tripRepository.CancelOrder(request);
        }

        public override ShippingList CreateUpdateShipmentList(int stopPointId, ShippingList request)
        {
            ShipmentListRequest shipmentListRequest = new ShipmentListRequest()
            {
                Requests = new List<ShipmentListDetails>()
            };

          
            foreach (string packingSheetNo in request.PKG_List)
            {
                int.TryParse(request.Colie.Trim(), out int numberOfBoxes);

                ShipmentListDetails shipmentListDetails = new ShipmentListDetails
                {
                    ShippingListNo = request.SL_No,
                    Note = "Pack",
                    PackingSheetNumber = packingSheetNo,
                    StopPointId = stopPointId,
                    NumberOfBoxes = numberOfBoxes
                };

                shipmentListRequest.Requests.Add(shipmentListDetails);
            }

            #region Insert PackagingSheet  to TMS 
            if (shipmentListRequest.Requests.Count > 0)
            {
                shipmentListRequest.OrderNumber = GetOrderNumber(stopPointId);
                shipmentListRequest.SequenceNumber = Convert.ToString(GetOrderSequnceNumber(stopPointId));
                LoginRequest loginRequest = new LoginRequest();
                string token = "";

                loginRequest.UserName = ConfigurationManager.AppSettings["TMSLogin"];
                loginRequest.UserPassword = ConfigurationManager.AppSettings["TMSPassword"];
                var tmsLoginResponse = JsonConvert.DeserializeObject<UserResponse>(Utility.GetApiResponse(ConfigurationManager.AppSettings["ApiGatewayTMSURL"]
                    + "/v1/user/login", Method.POST, loginRequest, null));
                if (tmsLoginResponse != null && tmsLoginResponse.Data.Count > 0)
                {
                    token = tmsLoginResponse.TokenKey;
                }
                #endregion

                var response = JsonConvert.DeserializeObject<UserResponse>(Utility.GetApiResponse(ConfigurationManager.AppSettings["ApiGatewayTMSURL"] + "/v1/Order/createUpdatePackingSheetDetailsDSM", Method.POST, shipmentListRequest,token));

            }
            return request;
           // return _tripRepository.CreateUpdateShipmentList(shipmentListRequest);
        }

        public override StopPointsResponse SwapeStopPoints(UpdateTripStatusRequest updateTripStatusRequest)
        {
            StopPointsResponse stopPointsResponse = _tripRepository.SwapeStopPoints(updateTripStatusRequest);

            #region Swape Stop Points in TMS 
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
                if (requestData.SequenceNumber != requestData.NewSequenceNumber)
                {
                    tmsRequest.Requests.Add(requestData);
                }

            }

            if (tmsRequest.Requests.Count > 0)
            {
                #region Call TMS API to Swape Stop Points in TMS 
                #region Login to TMS and get Token
                LoginRequest loginRequest = new LoginRequest();
                string token = "";

                loginRequest.UserName = ConfigurationManager.AppSettings["TMSLogin"];
                loginRequest.UserPassword = ConfigurationManager.AppSettings["TMSPassword"];
                var tmsLoginResponse = JsonConvert.DeserializeObject<UserResponse>(Utility.GetApiResponse(ConfigurationManager.AppSettings["ApiGatewayTMSURL"]
                    + "/v1/user/login", Method.POST, loginRequest, null));
                if (tmsLoginResponse != null && tmsLoginResponse.Data.Count > 0)
                {
                    token = tmsLoginResponse.TokenKey;
                }
                #endregion

                var response = JsonConvert.DeserializeObject<OrderStatusResponse>(Utility.GetApiResponse(ConfigurationManager.AppSettings["ApiGatewayTMSURL"]
                    + "/v1/order/swapestoppoints", Method.POST, tmsRequest, token));
                if (response != null)
                {
                    stopPointsResponse.StatusMessage += ". " + response.StatusMessage;
                }
                #endregion
            }
            #endregion
            return stopPointsResponse;
        }
    }
}
