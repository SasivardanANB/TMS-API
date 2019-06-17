using Newtonsoft.Json;
using NLog;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using TMS.API.Classes;
using TMS.DomainGateway.Gateway.Interfaces;
using TMS.DomainGateway.Task.Interfaces;
using TMS.DomainObjects.Objects;
using TMS.DomainObjects.Request;
using TMS.DomainObjects.Response;

namespace TMS.API.Controllers
{
    [CustomAuthorize]
    [RoutePrefix("api/v1/order")]
    public class OrderController : ApiController
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

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

        private Partner GetPartnerDetail(string partnerNo, int uploadType)
        {
            IOrderTask orderTask = Helper.Model.DependencyResolver.DependencyResolver.GetImplementationOf<ITaskGateway>().OrderTask;
            return orderTask.GetPartnerDetail(partnerNo, uploadType);
        }

        private string GetBusinessAreaCode(int businessAreaId)
        {
            IOrderTask orderTask = Helper.Model.DependencyResolver.DependencyResolver.GetImplementationOf<ITaskGateway>().OrderTask;
            return orderTask.GetBusinessAreaCode(businessAreaId);
        }
        #endregion

        [Route("createupdateorder")]
        [HttpPost]
        public IHttpActionResult CreateUpdateOrder(OrderRequest order)
        {
            for (int i = 0; i < order.Requests.Count; i++)
            {
                if (order.Requests[i].OrderType == 1) //For Inbound
                {
                    if (order.Requests[i].OrderWeight == 0)
                    {
                        ModelState.AddModelError($"{nameof(order)}.{nameof(order.Requests)}.[{i}].{nameof(Order.OrderWeight)}", "Invalid Order Weight");
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(order.Requests[i].OrderWeightUM))
                        {
                            ModelState.AddModelError($"{nameof(order)}.{nameof(order.Requests)}.[{i}].{nameof(Order.OrderWeightUM)}", "Invalid Order Weight UM");
                        }
                    }
                    if (order.UploadType == 1) // Upload Order
                    {
                        ModelState.Remove("order.Requests[" + i + "].BusinessAreaID");
                        ModelState.Remove("order.Requests[" + i + "]");

                        if (string.IsNullOrEmpty(order.Requests[i].ShippingListNo))
                        {
                            ModelState.AddModelError($"{nameof(order)}.{nameof(order.Requests)}.[{i}].{nameof(Order.ShippingListNo)}", "Invalid Shipping List Number");
                        }
                        if (string.IsNullOrEmpty(order.Requests[i].PackingSheetNo))
                        {
                            ModelState.AddModelError($"{nameof(order)}.{nameof(order.Requests)}.[{i}].{nameof(Order.PackingSheetNo)}", "Invalid Packing Sheet Number");
                        }
                        if (!string.IsNullOrEmpty(order.Requests[i].PackingSheetNo))
                        {
                            string[] packingSheets = order.Requests[i].PackingSheetNo.Split(',');
                            foreach (string packingSheet in packingSheets)
                            {
                                if(packingSheet.Length > 20)
                                {
                                    ModelState.AddModelError($"{nameof(order)}.{nameof(order.Requests)}.[{i}].{nameof(Order.PackingSheetNo)}", "Packing Sheet Number should not exceed 20 characters");
                                }
                            }
                        }
                        if (order.Requests[i].TotalCollie == 0)
                        {
                            ModelState.AddModelError($"{nameof(order)}.{nameof(order.Requests)}.[{i}].{nameof(Order.TotalCollie)}", "Invalid Total Collie");
                        }
                    }
                    else if (order.UploadType == 2) // Create Order
                    {
                        ModelState.Remove("order.Requests[" + i + "].BusinessArea");
                        ModelState.Remove("order.Requests[" + i + "].ShippingListNo");
                        ModelState.Remove("order.Requests[" + i + "].PackingSheetNo");
                        ModelState.Remove("order.Requests[" + i + "].TotalCollie");
                        ModelState.Remove("order.Requests[" + i + "].PartnerName1");
                        ModelState.Remove("order.Requests[" + i + "].PartnerName2");
                        ModelState.Remove("order.Requests[" + i + "].PartnerName3");
                        ModelState.Remove("order.Requests[" + i + "].Dimension");
                        ModelState.Remove("order.Requests[" + i + "].OrderNo");
                    }
                    else
                    {
                        ModelState.AddModelError($"{nameof(order)}.{nameof(order.Requests)}.[{i}].{nameof(Order.OrderType)}", "Invalid Upload Type");
                    }
                }
                else if (order.Requests[i].OrderType == 2)//For Outbound
                {
                    ModelState.Remove("order.Requests[" + i + "]");
                    ModelState.Remove("order.Requests[" + i + "].BusinessArea");

                    if (string.IsNullOrEmpty(order.Requests[i].ShipmentSAPNo))
                        ModelState.AddModelError($"{nameof(order)}.{nameof(order.Requests)}.[{i}].{nameof(Order.ShipmentSAPNo)}", "Invalid Shipment SAP Number");
                    if (order.UploadType == 1) // Upload Order
                    {
                        ModelState.Remove("order.Requests[" + i + "].BusinessAreaID");
                        ModelState.Remove("order.Requests[" + i + "]");

                        //if (string.IsNullOrEmpty(order.Requests[i].ShippingListNo))
                        //{
                        //    ModelState.AddModelError($"{nameof(order)}.{nameof(order.Requests)}.[{i}].{nameof(Order.ShippingListNo)}", "Invalid Shipping List Number");
                        //}
                        //if (string.IsNullOrEmpty(order.Requests[i].PackingSheetNo))
                        //{
                        //    ModelState.AddModelError($"{nameof(order)}.{nameof(order.Requests)}.[{i}].{nameof(Order.PackingSheetNo)}", "Invalid Packing Sheet Number");
                        //}
                        //if (!string.IsNullOrEmpty(order.Requests[i].PackingSheetNo))
                        //{
                        //    string[] packingSheets = order.Requests[i].PackingSheetNo.Split(',');
                        //    foreach (string packingSheet in packingSheets)
                        //    {
                        //        if (packingSheet.Length > 20)
                        //        {
                        //            ModelState.AddModelError($"{nameof(order)}.{nameof(order.Requests)}.[{i}].{nameof(Order.PackingSheetNo)}", "Packing Sheet Number should not exceed 20 characters");
                        //        }
                        //    }
                        //}
                        if (order.Requests[i].TotalCollie == 0)
                        {
                            ModelState.AddModelError($"{nameof(order)}.{nameof(order.Requests)}.[{i}].{nameof(Order.TotalCollie)}", "Invalid Total Collie");
                        }
                    }
                    else if (order.UploadType == 2) // Create Order
                    {
                        ModelState.Remove("order.Requests[" + i + "].BusinessArea");
                        ModelState.Remove("order.Requests[" + i + "].ShippingListNo");
                        ModelState.Remove("order.Requests[" + i + "].PackingSheetNo");
                        ModelState.Remove("order.Requests[" + i + "].TotalCollie");
                        ModelState.Remove("order.Requests[" + i + "].PartnerName1");
                        ModelState.Remove("order.Requests[" + i + "].PartnerName2");
                        ModelState.Remove("order.Requests[" + i + "].PartnerName3");
                        ModelState.Remove("order.Requests[" + i + "].Dimension");
                        ModelState.Remove("order.Requests.[" + i + "].ShipmentSAPNo");
                        ModelState.Remove("order.Requests[" + i + "].OrderNo");
                    }
                    else
                    {
                        ModelState.AddModelError($"{nameof(order)}.{nameof(order.Requests)}.[{i}].{nameof(Order.OrderType)}", "Invalid Upload Type");
                    }
                }
                else
                {
                    ModelState.AddModelError($"{nameof(order)}.{nameof(order.Requests)}.[{i}].{nameof(Order.OrderType)}", "Invalid Order Type");
                }
            }
            if (!ModelState.IsValid)
            {
                ErrorResponse errorResponse = new ErrorResponse()
                {
                    Status = DomainObjects.Resource.ResourceData.Failure,
                    StatusCode = (int)HttpStatusCode.BadRequest,
                    Data = new List<Error>()
                };
                for (int i = 0; i < ModelState.Keys.Count; i++)
                {
                    Error errorData = new Error()
                    {
                        ErrorMessage = ModelState.Keys.ToList<string>()[i].Replace("request.Requests[", "Row Number[") + " : " + ModelState.Values.ToList<ModelState>()[i].Errors[0].ErrorMessage
                    };

                    errorResponse.Data.Add(errorData);
                }
                return Ok(errorResponse);
            }

            IOrderTask orderTask = Helper.Model.DependencyResolver.DependencyResolver.GetImplementationOf<ITaskGateway>().OrderTask;
            OrderResponse orderData = orderTask.CreateUpdateOrder(order);

            if (orderData.StatusCode == 200 && orderData.Status == "Success")
            {
                #region Call DMS API to send Order as Trip if Driver assignment exists
                TripRequestDMS requestDMS = new TripRequestDMS()
                {
                    Requests = new List<TripDMS>()
                };

                foreach (var request in order.Requests)
                {
                    if (!string.IsNullOrEmpty(request.DriverName))
                    {
                        DateTime estimationShipmentDate = DateTime.ParseExact(request.EstimationShipmentDate, "dd.MM.yyyy", CultureInfo.InvariantCulture) + TimeSpan.Parse(request.EstimationShipmentTime);
                        DateTime actualShipmentDate = DateTime.ParseExact(request.ActualShipmentDate, "dd.MM.yyyy", CultureInfo.InvariantCulture) + TimeSpan.Parse(request.ActualShipmentTime);

                        if (requestDMS.Requests.Count > 0)
                        {
                            var existingTrip = requestDMS.Requests.FirstOrDefault(t => t.OrderNumber == request.OrderNo);
                            if (existingTrip != null)
                            {
                                if (request.OrderType == 1)
                                {
                                    #region Add Source Location
                                    Partner sourcePartnerDetail = GetPartnerDetail(request.PartnerNo2, order.UploadType);
                                    TripLocation sourceLocation = new TripLocation()
                                    {
                                        PartnerType = request.PartnerType2,
                                        PartnerNo = sourcePartnerDetail.PartnerNo, //request.PartnerNo2, 
                                        PartnerName = request.PartnerName2 == null ? sourcePartnerDetail.PartnerName : request.PartnerName2,
                                        SequnceNumber = request.SequenceNo,
                                        ActualDeliveryDate = actualShipmentDate,
                                        EstimatedDeliveryDate = estimationShipmentDate
                                    };
                                    existingTrip.TripLocations.Add(sourceLocation);
                                    requestDMS.Requests.Remove(existingTrip);
                                    requestDMS.Requests.Add(existingTrip);
                                    #endregion
                                }
                                else if (request.OrderType == 2)
                                {
                                    #region Add Destination Location

                                    Partner destinationPartnerDetail = GetPartnerDetail(request.PartnerNo3, order.UploadType);

                                    TripLocation destinationLocation = new TripLocation()
                                    {
                                        PartnerType = request.PartnerType3,
                                        PartnerNo = destinationPartnerDetail.PartnerNo ,//request.PartnerNo3,
                                        PartnerName = request.PartnerName3 == null ? destinationPartnerDetail.PartnerName : request.PartnerName3,
                                        SequnceNumber = request.SequenceNo,
                                        ActualDeliveryDate = actualShipmentDate,
                                        EstimatedDeliveryDate = estimationShipmentDate
                                    };
                                    existingTrip.TripLocations.Add(destinationLocation);
                                    requestDMS.Requests.Remove(existingTrip);
                                    requestDMS.Requests.Add(existingTrip);
                                    #endregion
                                }
                            }
                            else
                            {
                                string businessArea = "";
                                if (string.IsNullOrEmpty(request.BusinessArea))
                                    businessArea = GetBusinessAreaCode(request.BusinessAreaId);
                                else
                                    businessArea = request.BusinessArea;
                                Partner transporterPartnerDetail = GetPartnerDetail(request.PartnerNo1, order.UploadType);
                                TripDMS tripDMS = new TripDMS()
                                {
                                    OrderNumber = request.OrderNo,
                                    TransporterName = transporterPartnerDetail.PartnerName,
                                    TransporterCode = transporterPartnerDetail.PartnerNo,
                                    DriverName = request.DriverName,
                                    VehicleType = request.VehicleShipmentType,
                                    VehicleNumber = request.VehicleNo,
                                    TripType = Convert.ToString(request.FleetType),
                                    Weight = request.OrderWeight,
                                    PoliceNumber = request.VehicleNo,
                                    TripStatusCode = "3",
                                    OrderType = request.OrderType,
                                    BusinessAreaCode = businessArea,
                                    ShipmentScheduleImageGUID = request.ShipmentScheduleImageGUID,
                                    TripLocations = new List<TripLocation>()
                                };

                                #region Add Source Location
                                Partner sourcePartnerDetail = GetPartnerDetail(request.PartnerNo2, order.UploadType);
                                TripLocation sourceLocation = new TripLocation()
                                {
                                    PartnerType = request.PartnerType2,
                                    PartnerNo = request.PartnerNo2,
                                    PartnerName = request.PartnerName2 == null ? sourcePartnerDetail.PartnerName : request.PartnerName2,
                                    SequnceNumber = request.OrderType == 1 ? request.SequenceNo : 0,
                                    ActualDeliveryDate = actualShipmentDate,
                                    EstimatedDeliveryDate = estimationShipmentDate
                                };
                                tripDMS.TripLocations.Add(sourceLocation);
                                #endregion

                                #region Add Destination Location
                                Partner destinationPartnerDetail = GetPartnerDetail(request.PartnerNo3, order.UploadType);
                                TripLocation destinationLocation = new TripLocation()
                                {
                                    PartnerType = request.PartnerType3,
                                    PartnerNo = request.PartnerNo3,
                                    PartnerName = request.PartnerName3 == null ? destinationPartnerDetail.PartnerName : request.PartnerName3,
                                    SequnceNumber = request.OrderType == 1 ? 0 : request.SequenceNo,
                                    ActualDeliveryDate = actualShipmentDate,
                                    EstimatedDeliveryDate = estimationShipmentDate
                                };
                                tripDMS.TripLocations.Add(destinationLocation);
                                requestDMS.Requests.Add(tripDMS);
                                #endregion
                            }
                        }
                        else
                        {
                            string businessArea = "";
                            if (string.IsNullOrEmpty(request.BusinessArea))
                                businessArea = GetBusinessAreaCode(request.BusinessAreaId);
                            else
                                businessArea = request.BusinessArea;
                            Partner transporterPartnerDetail = GetPartnerDetail(request.PartnerNo1, order.UploadType);

                            TripDMS tripDMS = new TripDMS()
                            {
                                OrderNumber = request.OrderNo,
                                TransporterName = transporterPartnerDetail.PartnerName,
                                TransporterCode = transporterPartnerDetail.PartnerNo,
                                DriverNo = request.DriverNo,
                                DriverName = request.DriverName,
                                VehicleType = request.VehicleShipmentType,
                                VehicleNumber = request.VehicleNo,
                                TripType = Convert.ToString(request.FleetType),
                                Weight = request.OrderWeight,
                                PoliceNumber = request.VehicleNo,
                                TripStatusCode = "3",
                                OrderType = request.OrderType,
                                BusinessAreaCode = businessArea,
                                ShipmentScheduleImageGUID = request.ShipmentScheduleImageGUID,
                                TripLocations = new List<TripLocation>()
                            };

                            #region Add Source Location
                            Partner sourcePartnerDetail = GetPartnerDetail(request.PartnerNo2, order.UploadType);
                            TripLocation sourceLocation = new TripLocation()
                            {
                                PartnerType = request.PartnerType2,
                                PartnerNo = sourcePartnerDetail.PartnerNo,// request.PartnerNo2,
                                PartnerName = request.PartnerName2 == null ? sourcePartnerDetail.PartnerName : request.PartnerName2,
                                SequnceNumber = request.OrderType == 1 ? request.SequenceNo : 0,
                                ActualDeliveryDate = actualShipmentDate,
                                EstimatedDeliveryDate = estimationShipmentDate
                            };
                            tripDMS.TripLocations.Add(sourceLocation);
                            #endregion

                            #region Add Destination Location
                            Partner destinationPartnerDetail = GetPartnerDetail(request.PartnerNo3, order.UploadType);
                            TripLocation destinationLocation = new TripLocation()
                            {
                                PartnerType = request.PartnerType3,
                                PartnerNo = destinationPartnerDetail.PartnerNo,// request.PartnerNo3,
                                PartnerName = request.PartnerName3 == null ? destinationPartnerDetail.PartnerName : request.PartnerName3,
                                SequnceNumber = request.OrderType == 1 ? 0 : request.SequenceNo,
                                ActualDeliveryDate = actualShipmentDate,
                                EstimatedDeliveryDate = estimationShipmentDate
                            };
                            tripDMS.TripLocations.Add(destinationLocation);
                            #endregion

                            requestDMS.Requests.Add(tripDMS);
                        }
                    }
                }
                #endregion
                if (requestDMS.Requests.Count > 0)
                {
                    #region Call DMS API to assign order to driver
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

                    var response = JsonConvert.DeserializeObject<UserResponse>(GetApiResponse(ConfigurationManager.AppSettings["ApiGatewayDMSURL"]
                        + "/v1/trip/createupdatetrip", Method.POST, requestDMS, token));
                    if (response != null)
                    {
                        orderData.StatusMessage += ". " + response.StatusMessage;
                    }
                    #endregion
                }
            }
            return Ok(orderData);
        }

        [Route("getorders")]
        [AllowAnonymous, HttpPost]
        public IHttpActionResult GetOrders(OrderSearchRequest orderSearchRequest)
        {
            IOrderTask orderTask = Helper.Model.DependencyResolver.DependencyResolver.GetImplementationOf<ITaskGateway>().OrderTask;
            OrderSearchResponse orderSearchResponse = orderTask.GetOrders(orderSearchRequest);
            return Ok(orderSearchResponse);
        }

        [Route("createupdatepackingsheet")]
        [HttpPost]
        public IHttpActionResult CreateUpdatePackingSheet(PackingSheetRequest packingSheetRequest)
        {
            IOrderTask orderTask = Helper.Model.DependencyResolver.DependencyResolver.GetImplementationOf<ITaskGateway>().OrderTask;
            PackingSheetResponse packingSheetResponse = orderTask.CreateUpdatePackingSheet(packingSheetRequest);
            return Ok(packingSheetResponse);
        }

        [Route("getpackingsheetdetails")]
        [HttpGet]
        public IHttpActionResult GetPackingSheetDetails(int orderId)
        {
            IOrderTask orderTask = Helper.Model.DependencyResolver.DependencyResolver.GetImplementationOf<ITaskGateway>().OrderTask;
            PackingSheetResponse packingSheetResponse = orderTask.GetPackingSheetDetails(orderId);
            return Ok(packingSheetResponse);
        }


        [Route("trackorder")]
        [HttpGet]
        public IHttpActionResult TrackOrder(int orderId)
        {
            IOrderTask orderTask = Helper.Model.DependencyResolver.DependencyResolver.GetImplementationOf<ITaskGateway>().OrderTask;
            OrderTrackResponse orderTrackResponse = orderTask.TrackOrder(orderId);
            return Ok(orderTrackResponse);
        }

        [Route("getorderids")]
        [HttpGet]
        public IHttpActionResult GetOrderIds()
        {
            IOrderTask orderTask = Helper.Model.DependencyResolver.DependencyResolver.GetImplementationOf<ITaskGateway>().OrderTask;
            CommonResponse commonResponse = orderTask.GetOrderIds();
            return Ok(commonResponse);
        }

        [Route("getdealers")]
        [AllowAnonymous, HttpGet]
        public IHttpActionResult GetDealers(int orderId, string searchText = "")
        {
            IOrderTask orderTask = Helper.Model.DependencyResolver.DependencyResolver.GetImplementationOf<ITaskGateway>().OrderTask;
            DealerDetailsResponse dealerDetailsResponse = orderTask.GetDealers(orderId, searchText);
            return Ok(dealerDetailsResponse);
        }

        [Route("getorderdetails")]
        [HttpGet]
        public IHttpActionResult GetOrderDetails(int orderId)
        {
            IOrderTask orderTask = Helper.Model.DependencyResolver.DependencyResolver.GetImplementationOf<ITaskGateway>().OrderTask;
            OrderDetailsResponse orderDetailsResponse = orderTask.GetOrderDetails(orderId);
            return Ok(orderDetailsResponse);
        }

        [Route("updateorderstatus")]
        [HttpPost]
        public IHttpActionResult UpdateOrderStatus(OrderStatusRequest request)
        {
            IOrderTask orderTask = Helper.Model.DependencyResolver.DependencyResolver.GetImplementationOf<ITaskGateway>().OrderTask;
            OrderStatusResponse response = orderTask.UpdateOrderStatus(request);
            return Ok(response);
        }

    }
}
