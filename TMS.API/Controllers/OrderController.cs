using ActiveUp.Net.Mail;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using TMS.API.Classes;
using TMS.DomainGateway.Gateway.Interfaces;
using TMS.DomainGateway.Task.Interfaces;
using TMS.DomainObjects.Objects;
using TMS.DomainObjects.Request;
using TMS.DomainObjects.Response;
using static TMS.API.Controllers.MediaController;

namespace TMS.API.Controllers
{
    [CustomAuthorize]
    [RoutePrefix("api/v1/order")]
    public class OrderController : ApiController
    {
        private readonly NLog.Logger _logger = LogManager.GetCurrentClassLogger();

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

        private static string GetFileUploadApiResponse(string apiRoute, Method method, MimePart mimePart, string token)
        {
            var client = new RestClient(ConfigurationManager.AppSettings["ApiGatewayBaseURL"]);
            if (token != null)
                client.AddDefaultHeader("Token", token);
            var request = new RestRequest(apiRoute, method) { RequestFormat = DataFormat.Json, AlwaysMultipartFormData = true };
            request.Timeout = 500000;
            if (mimePart != null)
            {
                client.AddDefaultHeader("Content-Type", "multipart/form-data");
                request.AddFileBytes("file", mimePart.BinaryContent, mimePart.Filename, mimePart.ContentType.MimeType);// upload from file byte array
            }
            var result = client.Execute(request);
            return result.Content;
        }

        private Partner GetPartnerDetail(string partnerNo, int uploadType)
        {
            IOrderTask orderTask = Helper.Model.DependencyResolver.DependencyResolver.GetImplementationOf<ITaskGateway>().OrderTask;
            return orderTask.GetPartnerDetail(partnerNo, uploadType);
        }

        private OrderResponse OcrOrderResponse(ShipmentScheduleOcrRequest  shipmentScheduleOcrRequest)
        {
            IOrderTask orderTask = Helper.Model.DependencyResolver.DependencyResolver.GetImplementationOf<ITaskGateway>().OrderTask;
            return orderTask.OcrOrderResponse(shipmentScheduleOcrRequest);
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
                                if (packingSheet.Length > 20)
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

            //Login to OMS and get Token
            LoginRequest omsLoginRequest = new LoginRequest();
            string omsToken = "";
            omsLoginRequest.UserName = ConfigurationManager.AppSettings["OMSLogin"];
            omsLoginRequest.UserPassword = ConfigurationManager.AppSettings["OMSPassword"];
            var tmsLoginResponse = JsonConvert.DeserializeObject<UserResponse>(GetApiResponse(ConfigurationManager.AppSettings["ApiGatewayOMSURL"]
                                                                                              + "v1/user/login", Method.POST, omsLoginRequest, null));
            if (tmsLoginResponse != null && tmsLoginResponse.Data.Count > 0)
            {
                omsToken = tmsLoginResponse.TokenKey;
            }

            // Prepare OMSRequest
            OrderRequest omsRequest = new OrderRequest()
            {
                Requests = new List<Order>(),
                CreatedBy = "TMS System",
                orderGeneratedSystem = "TMS"  // Useful for creating order only on OMS
            };


            OrderResponse omsOrderResponse = new OrderResponse();
            OrderResponse tmsOrderResponse = new OrderResponse();
            if (order.UploadType == 1) // Excel
            {
                omsRequest.UploadType = 1;

                if (order.orderGeneratedSystem != "OMS") // TMS
                {
                    foreach (var tmsOrder in order.Requests)
                    {
                        Partner partner1Data = GetPartnerDetail(tmsOrder.PartnerNo1, order.UploadType);
                        Partner partner2Data = GetPartnerDetail(tmsOrder.PartnerNo2, order.UploadType);
                        Partner partner3Data = GetPartnerDetail(tmsOrder.PartnerNo3, order.UploadType);

                        Order omsOrder = new Order()
                        {
                            BusinessArea = tmsOrder.BusinessArea,
                            OrderNo = tmsOrder.OrderNo,
                            SequenceNo = tmsOrder.SequenceNo,
                            PartnerNo1 = partner1Data.PartnerNo,
                            PartnerType1 = tmsOrder.PartnerType1,
                            PartnerName1 = partner1Data.PartnerName,
                            PartnerNo2 = partner2Data.PartnerNo,
                            PartnerType2 = tmsOrder.PartnerType2,
                            PartnerName2 = partner2Data.PartnerName,
                            PartnerNo3 = partner3Data.PartnerNo,
                            PartnerType3 = tmsOrder.PartnerType3,
                            PartnerName3 = partner3Data.PartnerName,
                            FleetType = tmsOrder.FleetType,
                            OrderType = tmsOrder.OrderType,
                            VehicleShipmentType = tmsOrder.VehicleShipmentType,
                            DriverNo = tmsOrder.DriverNo,
                            DriverName = tmsOrder.DriverName,
                            VehicleNo = tmsOrder.VehicleNo,
                            OrderWeight = tmsOrder.OrderWeight,
                            OrderWeightUM = tmsOrder.OrderWeightUM,
                            EstimationShipmentDate = tmsOrder.EstimationShipmentDate,
                            EstimationShipmentTime = tmsOrder.EstimationShipmentTime,
                            ActualShipmentDate = tmsOrder.ActualShipmentDate,
                            ActualShipmentTime = tmsOrder.ActualShipmentTime,
                            Sender = tmsOrder.Sender,
                            Receiver = tmsOrder.Receiver,
                            OrderShipmentStatus = tmsOrder.OrderShipmentStatus,
                            Dimension = tmsOrder.Dimension,
                            TotalPallet = tmsOrder.TotalPallet,
                            Instructions = tmsOrder.Instructions,
                            ShippingListNo = tmsOrder.ShippingListNo,
                            PackingSheetNo = tmsOrder.PackingSheetNo,
                            TotalCollie = tmsOrder.TotalCollie,
                            ShipmentSAPNo = tmsOrder.ShipmentSAPNo
                        };
                        omsRequest.Requests.Add(omsOrder);
                    }
                    omsOrderResponse = JsonConvert.DeserializeObject<OrderResponse>(GetApiResponse(ConfigurationManager.AppSettings["ApiGatewayOMSURL"]
                                                                                                              + "v1/order/createupdateorders", Method.POST, omsRequest, omsToken));
                }

                if ((order.orderGeneratedSystem != "OMS" && omsOrderResponse.StatusCode == (int)HttpStatusCode.OK) || order.orderGeneratedSystem == "OMS")
                {
                    // Create Order in TMS
                    IOrderTask tmsOrderTask = Helper.Model.DependencyResolver.DependencyResolver.GetImplementationOf<ITaskGateway>().OrderTask;
                    tmsOrderResponse = tmsOrderTask.CreateUpdateOrder(order);

                    if (order.orderGeneratedSystem != "OMS")
                    {
                        omsOrderResponse.StatusMessage = omsOrderResponse.StatusMessage + ". " + tmsOrderResponse.StatusMessage;
                    }

                    if (tmsOrderResponse.StatusCode == 200 && tmsOrderResponse.Status == "Success")
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
                                                PartnerNo = sourcePartnerDetail.PartnerNo,
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
                                                PartnerNo = destinationPartnerDetail.PartnerNo,//request.PartnerNo3,
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
                                            DriverNo = request.DriverNo,
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
                                if (order.orderGeneratedSystem != "OMS") // TMS
                                {
                                    omsOrderResponse.StatusMessage += ". " + response.StatusMessage;
                                }
                                else
                                {
                                    tmsOrderResponse.StatusMessage += ". " + response.StatusMessage;
                                }
                            }
                            #endregion
                        }
                    }
                }
            }
            else
            {
                omsRequest.UploadType = 2;

                DriverRequest driverRequest = new DriverRequest();
                driverRequest.Requests = new List<Driver>();
                driverRequest.Requests.Add(new Driver
                {
                    DriverNo = order.Requests[0].DriverNo
                });

                string tmsToken = string.Empty;
                if (Request.Headers.Contains("Token"))
                {
                    tmsToken = Request.Headers.GetValues("Token").First();
                }

                var driverData = JsonConvert.DeserializeObject<DriverResponse>(GetApiResponse(ConfigurationManager.AppSettings["ApiGatewayTMSURL"]
                                                                                                          + "v1/driver/getdrivers", Method.POST, driverRequest, tmsToken));
                string driverName = string.Empty; string driverNumber = string.Empty;
                if (driverData.StatusCode == (int)HttpStatusCode.OK && driverData.Status == DomainObjects.Resource.ResourceData.Success)
                {
                    driverName = driverData.Data[0].UserName;
                    driverNumber = driverData.Data[0].DriverNo;
                }


                foreach (var tmsOrder in order.Requests)
                {
                    string businessArea = "";
                    if (string.IsNullOrEmpty(tmsOrder.BusinessArea))
                        businessArea = GetBusinessAreaCode(tmsOrder.BusinessAreaId);
                    else
                        businessArea = tmsOrder.BusinessArea;
                    Partner partner1Data = GetPartnerDetail(tmsOrder.PartnerNo1, order.UploadType);
                    Partner partner2Data = GetPartnerDetail(tmsOrder.PartnerNo2, order.UploadType);
                    Partner partner3Data = GetPartnerDetail(tmsOrder.PartnerNo3, order.UploadType);

                    Order omsOrder = new Order()
                    {
                        BusinessArea = businessArea,
                        OrderNo = tmsOrder.OrderNo,
                        SequenceNo = tmsOrder.SequenceNo,
                        PartnerNo1 = partner1Data.PartnerNo,
                        PartnerType1 = tmsOrder.PartnerType1,
                        PartnerName1 = partner1Data.PartnerName,
                        PartnerNo2 = partner2Data.PartnerNo,
                        PartnerType2 = tmsOrder.PartnerType2,
                        PartnerName2 = partner2Data.PartnerName,
                        PartnerNo3 = partner3Data.PartnerNo,
                        PartnerType3 = tmsOrder.PartnerType3,
                        PartnerName3 = partner3Data.PartnerName,
                        FleetType = tmsOrder.FleetType,
                        OrderType = tmsOrder.OrderType,
                        VehicleShipmentType = tmsOrder.VehicleShipmentType,
                        DriverNo = driverNumber,
                        DriverName = driverName,
                        VehicleNo = tmsOrder.VehicleNo,
                        OrderWeight = tmsOrder.OrderWeight,
                        OrderWeightUM = tmsOrder.OrderWeightUM,
                        EstimationShipmentDate = tmsOrder.EstimationShipmentDate,
                        EstimationShipmentTime = tmsOrder.EstimationShipmentTime,
                        ActualShipmentDate = tmsOrder.ActualShipmentDate,
                        ActualShipmentTime = tmsOrder.ActualShipmentTime,
                        Sender = tmsOrder.Sender,
                        Receiver = tmsOrder.Receiver,
                        OrderShipmentStatus = tmsOrder.OrderShipmentStatus,
                        Dimension = tmsOrder.Dimension,
                        TotalPallet = tmsOrder.TotalPallet,
                        Instructions = tmsOrder.Instructions,
                        ShippingListNo = tmsOrder.ShippingListNo,
                        PackingSheetNo = tmsOrder.PackingSheetNo,
                        TotalCollie = tmsOrder.TotalCollie,
                        ShipmentSAPNo = tmsOrder.ShipmentSAPNo
                    };
                    omsRequest.Requests.Add(omsOrder);
                }

                omsOrderResponse = JsonConvert.DeserializeObject<OrderResponse>(GetApiResponse(ConfigurationManager.AppSettings["ApiGatewayOMSURL"]
                                                                                                          + "v1/order/syncorders", Method.POST, omsRequest, omsToken));

                if (omsOrderResponse.StatusCode == (int)HttpStatusCode.OK)
                {
                    // Get orderNumber from oms response and update tmsRequest
                    foreach (var ord in order.Requests)
                    {
                        ord.OrderNo = omsOrderResponse.Data[0].OrderNo;
                        ord.LegecyOrderNo = omsOrderResponse.Data[0].OrderNo;
                    }

                    // Create Order in TMS
                    IOrderTask tmsOrderTask = Helper.Model.DependencyResolver.DependencyResolver.GetImplementationOf<ITaskGateway>().OrderTask;
                    tmsOrderResponse = tmsOrderTask.CreateUpdateOrder(order);

                    omsOrderResponse.StatusMessage = omsOrderResponse.StatusMessage + ". " + tmsOrderResponse.StatusMessage;

                    if (tmsOrderResponse.StatusCode == 200 && tmsOrderResponse.Status == "Success")
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
                                                PartnerNo = destinationPartnerDetail.PartnerNo,//request.PartnerNo3,
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
                                            DriverNo = request.DriverNo,
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
                                omsOrderResponse.StatusMessage += ". " + response.StatusMessage;
                            }
                            #endregion
                        }
                    }
                }
            }

            if (order.orderGeneratedSystem != "OMS") // TMS
            {
                return Ok(omsOrderResponse);
            }
            else
            {
                return Ok(tmsOrderResponse);
            }
        }

        [Route("getorders")]
        [AllowAnonymous, HttpPost]
        public IHttpActionResult GetOrders(OrderSearchRequest orderSearchRequest)
        {
            IEnumerable<string> headerValues;
            var tokenValue = string.Empty;
            if (Request.Headers.TryGetValues("Token", out headerValues))
            {
                tokenValue = headerValues.FirstOrDefault();
                orderSearchRequest.Token = tokenValue;
            }

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

            if (packingSheetResponse.Status == DomainObjects.Resource.ResourceData.Success && packingSheetResponse.StatusCode == (int)HttpStatusCode.OK && packingSheetResponse.Data.Count > 0)
            {

                foreach (PackingSheet ps in packingSheetRequest.Requests)
                {
                    ps.OrderNumber = packingSheetResponse.Data[0].OrderNumber;

                    ps.DealerNumber = (from pks in packingSheetResponse.Data
                                       where pks.DealerId == ps.DealerId
                                       select pks.DealerNumber).FirstOrDefault();
                    ps.DealerId = 0;
                    ps.OrderDetailId = 0;
                }

                #region Call OMS API to update packing sheet details

                LoginRequest omsLoginRequest = new LoginRequest();
                string omsToken = "";
                omsLoginRequest.UserName = ConfigurationManager.AppSettings["OMSLogin"];
                omsLoginRequest.UserPassword = ConfigurationManager.AppSettings["OMSPassword"];
                var tmsLoginResponse = JsonConvert.DeserializeObject<UserResponse>(GetApiResponse(ConfigurationManager.AppSettings["ApiGatewayOMSURL"]
                                                                                                  + "v1/user/login", Method.POST, omsLoginRequest, null));
                if (tmsLoginResponse != null && tmsLoginResponse.Data.Count > 0)
                {
                    omsToken = tmsLoginResponse.TokenKey;
                }

                PackingSheetResponse omsPackingSheetResponse = JsonConvert.DeserializeObject<PackingSheetResponse>(GetApiResponse(ConfigurationManager.AppSettings["ApiGatewayOMSURL"]
                                                                                                          + "v1/order/CreateUpdatePackingSheet", Method.POST, packingSheetRequest, omsToken));

                packingSheetResponse.StatusMessage = omsPackingSheetResponse.StatusMessage + " " + packingSheetResponse.StatusMessage;

                #endregion
            }

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
            IEnumerable<string> headerValues;
            string tokenValue = string.Empty;
            if (Request.Headers.TryGetValues("Token", out headerValues))
            {
                tokenValue = headerValues.FirstOrDefault().ToString();
            }

            IOrderTask orderTask = Helper.Model.DependencyResolver.DependencyResolver.GetImplementationOf<ITaskGateway>().OrderTask;
            CommonResponse commonResponse = orderTask.GetOrderIds(tokenValue);
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
            #region Update Status to OMS 
            OrderStatusRequest omsRequest = new OrderStatusRequest()
            {
                Requests = new List<OrderStatus>()
            };

            foreach (var item in request.Requests)
            {

                OrderStatus requestData = new OrderStatus()
                {
                    IsLoad = null,
                    OrderNumber = item.OrderNumber,
                    OrderStatusCode = item.OrderStatusCode,
                    Remarks = "",
                    SequenceNumber = item.SequenceNumber,
                    NewSequenceNumber = item.NewSequenceNumber
                };

                omsRequest.Requests.Add(requestData);
                omsRequest.RequestFrom = "TMS";
            }

            if (omsRequest.Requests.Count > 0)
            {
                #region Call OMS API to Update Order
                #region Login to OMS and get Token
                LoginRequest loginRequest = new LoginRequest();
                string token = "";

                loginRequest.UserName = ConfigurationManager.AppSettings["OMSLogin"];
                loginRequest.UserPassword = ConfigurationManager.AppSettings["OMSPassword"];
                var tmsLoginResponse = JsonConvert.DeserializeObject<UserResponse>(GetApiResponse(ConfigurationManager.AppSettings["ApiGatewayOMSURL"]
                    + "/v1/user/login", Method.POST, loginRequest, null));
                if (tmsLoginResponse != null && tmsLoginResponse.Data.Count > 0)
                {
                    token = tmsLoginResponse.TokenKey;
                }
                #endregion

                var omsresponse = JsonConvert.DeserializeObject<UserResponse>(GetApiResponse(ConfigurationManager.AppSettings["ApiGatewayOMSURL"]
                    + "/v1/order/updateorderstatus", Method.POST, omsRequest, token));
                if (response != null)
                {
                    response.StatusMessage += ". " + omsresponse.StatusMessage;
                }
                #endregion
            }
            #endregion
            #region Invoice Generation
            if (request.Requests[0].OrderStatusCode == "12")
            {
                InvoiceResponse invoiceResponse = orderTask.GetInvoiceRequest(request);
                InvoiceResponse invoiceResponseData = new InvoiceResponse();
                if(invoiceResponse != null && invoiceResponse.Data.Count > 0)
                {
                    InvoiceRequest invoiceRequest = new InvoiceRequest();
                    invoiceRequest.Requests = invoiceResponse.Data;
                    var tmsToken = Request.Headers.GetValues("Token").FirstOrDefault();
                    if (tmsToken != null)
                    {
                        invoiceResponseData = JsonConvert.DeserializeObject<InvoiceResponse>(GetApiResponse(ConfigurationManager.AppSettings["ApiGatewayTMSURL"]
                                                                                                                                   + "v1/invoice/generateinvoice", Method.POST, invoiceRequest, tmsToken));
                    }
                }
            }
            #endregion
            return Ok(response);
        }

        [Route("getshippinglistguids")]
        [HttpGet]
        public IHttpActionResult GetShippingListGuids(string orderNumber)
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

            var response = JsonConvert.DeserializeObject<ImageGuidsResponse>(GetApiResponse(ConfigurationManager.AppSettings["ApiGatewayDMSURL"]
                + "/v1/trip/getshippinglistguids?orderNumber=" + orderNumber, Method.GET, null, token));
            #endregion
            return Ok(response);
        }

        [Route("getpodguids")]
        [HttpGet]
        public IHttpActionResult GetPodGuids(string orderNumber)
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

            var response = JsonConvert.DeserializeObject<ImageGuidsResponse>(GetApiResponse(ConfigurationManager.AppSettings["ApiGatewayDMSURL"]
                + "/v1/trip/getpodguids?orderNumber=" + orderNumber, Method.GET, null, token));

            #endregion
            return Ok(response);
        }

        [Route("getphotowithcustomerguids")]
        [HttpGet]
        public IHttpActionResult GetPhotoWithCustomerGuids(string orderNumber)
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

            var response = JsonConvert.DeserializeObject<ImageGuidsResponse>(GetApiResponse(ConfigurationManager.AppSettings["ApiGatewayDMSURL"]
                + "/v1/trip/getphotowithcustomerguids?orderNumber=" + orderNumber, Method.GET, null, token));

            #endregion
            return Ok(response);
        }

        [Route("cancelorder")]
        [HttpPost]
        public IHttpActionResult CancelOrder(OrderStatusRequest request)
        {
            IOrderTask orderTask = Helper.Model.DependencyResolver.DependencyResolver.GetImplementationOf<ITaskGateway>().OrderTask;
            OrderStatusResponse response = orderTask.CancelOrder(request);

            #region Update Status to OMS 
            OrderStatusRequest omsRequest = new OrderStatusRequest()
            {
                Requests = new List<OrderStatus>()
            };

            foreach (var item in request.Requests)
            {
                OrderStatus requestData = new OrderStatus()
                {
                    IsLoad = null,
                    OrderNumber = item.OrderNumber,
                    OrderStatusCode = item.OrderStatusCode,
                    Remarks = "",
                    SequenceNumber = item.SequenceNumber,
                    NewSequenceNumber = item.NewSequenceNumber
                };
                omsRequest.Requests.Add(requestData);
                omsRequest.RequestFrom = "TMS";
            }
            if (omsRequest.Requests.Count > 0)
            {
                #region Call OMS API to Update Order
                #region Login to OMS and get Token
                LoginRequest loginRequest = new LoginRequest();
                string token = "";

                loginRequest.UserName = ConfigurationManager.AppSettings["OMSLogin"];
                loginRequest.UserPassword = ConfigurationManager.AppSettings["OMSPassword"];
                var tmsLoginResponse = JsonConvert.DeserializeObject<UserResponse>(GetApiResponse(ConfigurationManager.AppSettings["ApiGatewayOMSURL"]
                    + "/v1/user/login", Method.POST, loginRequest, null));
                if (tmsLoginResponse != null && tmsLoginResponse.Data.Count > 0)
                {
                    token = tmsLoginResponse.TokenKey;
                }
                #endregion

                var omsresponse = JsonConvert.DeserializeObject<UserResponse>(GetApiResponse(ConfigurationManager.AppSettings["ApiGatewayOMSURL"]
                    + "/v1/order/cancelorder", Method.POST, omsRequest, token));
                if (response != null)
                {
                    response.StatusMessage += ". " + omsresponse.StatusMessage;
                }
                #endregion
            }
            #endregion

            #region Update Status to DMS 
            OrderStatusRequest dmsRequest = new OrderStatusRequest()
            {
                Requests = new List<OrderStatus>()
            };

            foreach (var item in request.Requests)
            {
                OrderStatus requestData = new OrderStatus()
                {
                    IsLoad = null,
                    OrderNumber = item.OrderNumber,
                    OrderStatusCode = item.OrderStatusCode,
                    Remarks = "",
                    SequenceNumber = item.SequenceNumber,
                    NewSequenceNumber = item.NewSequenceNumber
                };
                dmsRequest.Requests.Add(requestData);
                dmsRequest.RequestFrom = "TMS";
            }
            if (dmsRequest.Requests.Count > 0)
            {
                #region Call DMS API to Update Order
                #region Login to DMS and get Token
                LoginRequest dmsLoginRequest = new LoginRequest();
                string dmsToken = "";

                dmsLoginRequest.UserName = ConfigurationManager.AppSettings["DMSLogin"];
                dmsLoginRequest.UserPassword = ConfigurationManager.AppSettings["DMSPassword"];
                var dmsLoginResponse = JsonConvert.DeserializeObject<UserResponse>(GetApiResponse(ConfigurationManager.AppSettings["ApiGatewayDMSURL"]
                    + "/v1/user/login", Method.POST, dmsLoginRequest, null));
                if (dmsLoginResponse != null && dmsLoginResponse.Data.Count > 0)
                {
                    dmsToken = dmsLoginResponse.TokenKey;
                }
                #endregion

                var dmsresponse = JsonConvert.DeserializeObject<UserResponse>(GetApiResponse(ConfigurationManager.AppSettings["ApiGatewayDMSURL"]
                    + "/v1/trip/cancelorder", Method.POST, omsRequest, dmsToken));
                if (response != null)
                {
                    response.StatusMessage += ". " + dmsresponse.StatusMessage;
                }
                #endregion
            }
            #endregion

            return Ok(response);
        }

        [Route("getharga")]
        [HttpPost]
        public IHttpActionResult GetHarga(HargaRequest request)
        {
            IOrderTask orderTask = Helper.Model.DependencyResolver.DependencyResolver.GetImplementationOf<ITaskGateway>().OrderTask;
            HargaResponse response = orderTask.GetHarga(request);
            return Ok(response);
        }

        [Route("getshipmentschedulesfromemail")]
        [HttpGet]
        public IHttpActionResult GetShipmentSchedulesFromEmail()
        {
            IOrderTask orderTask = Helper.Model.DependencyResolver.DependencyResolver.GetImplementationOf<ITaskGateway>().OrderTask;
            ShipmentScheduleOcrResponse response = new ShipmentScheduleOcrResponse();
            OrderResponse ocrOrderResponse = new OrderResponse();
            OrderResponse tmsOrderResponse = new OrderResponse();
            OrderResponse omsOrderResponse = new OrderResponse();
            // Loin into Gamil and Get all Mails
            var mailRepository = new MailRepository(
                                    ConfigurationManager.AppSettings["GmailURL"],
                                    Convert.ToInt32(ConfigurationManager.AppSettings["GmailURLPort"].ToString()),
                                    true,
                                    ConfigurationManager.AppSettings["UserEmailID"] ,
                                    ConfigurationManager.AppSettings["UserPassword"]
                                );

            //Get all Unread Mails
            var emailList = mailRepository.GetUnreadMails("inbox");

            foreach (ActiveUp.Net.Mail.Message email in emailList)
            {
                if (email.Attachments.Count > 0)
                {
                    foreach (MimePart attachment in email.Attachments)
                    {
                        if (attachment.ContentType.MimeType.ToLower() == "application/pdf") // Checking For PDF files
                        {
                            // Uploading File into Blob storage and get GUID
                            var fileuploadresponse = JsonConvert.DeserializeObject<ResponseDataForFileUpload>(GetFileUploadApiResponse(ConfigurationManager.AppSettings["ApiGatewayTMSURL"] + "/v1/media/uploadfile", Method.POST, attachment, null));
                           // response.StatusMessage += ". " + fileuploadresponse.Guid;

                            if (fileuploadresponse.StatusCode == (int)HttpStatusCode.OK && fileuploadresponse.Guid != "" && fileuploadresponse.Guid != null)
                            {
                                // Calling OCR to get shipmentschedule data
                                var res =  GetFileUploadApiResponse(ConfigurationManager.AppSettings["ApiGatewayTMSURL"] + "/v1/order/shipmentscheduleocr", Method.POST, attachment, null);
                                var shipmentScheduleOcr = JsonConvert.DeserializeObject<dynamic>(res);
                                var jsonObject = JObject.Parse(shipmentScheduleOcr);
                                ShipmentScheduleOcrRequest shipmentScheduleOcrRequest = new ShipmentScheduleOcrRequest();
                                if (jsonObject.success == true)
                                {
                                    //var docType = jsonObject.documentType;
                                  
                                    shipmentScheduleOcrRequest.Requests = new List<ShipmentScheduleOcr>();
                                    ShipmentScheduleOcr shipmentSchedule = new ShipmentScheduleOcr()
                                    {
                                        ImageGUID= fileuploadresponse.Guid,
                                        DocumentType = jsonObject.documentType,
                                        Success = jsonObject.success,
                                        Image = jsonObject.image,
                                        EmailFrom = email.From.Email,
                                        EmailDateTime=email.Date,
                                        EmailSubject=email.Subject,
                                        EmailText= email.BodyHtml.TextStripped,
                                        Data = new DetailsOcr() {
                                            DayShipment = jsonObject.data.DayShipment,
                                            EstimatedTotalPallet = jsonObject.data.EstimatedTotalPallet,
                                            MainDealerCode = jsonObject.data.MainDealerCode,
                                            MainDealerName= jsonObject.data.MainDealerName,
                                            MultiDropShipment= jsonObject.data.MultiDropShipment,
                                            ShipmentScheduleNo= jsonObject.data.ShipmentScheduleNo,
                                            ShipmentTime= jsonObject.data.ShipmentTime,
                                            ShipToParty= jsonObject.data.ShipToParty,
                                            VehicleType= jsonObject.data.VehicleType,
                                            Weight= jsonObject.data.Weight
                                        }
                                    };
                                    shipmentScheduleOcrRequest.Requests.Add(shipmentSchedule);

                                }
                                if(shipmentScheduleOcrRequest.Requests.Count > 0)
                                {
                                    //response = orderTask.CreateOrderFromShipmentScheduleOcr(shipmentScheduleOcrRequest);
                                    //OcrOrderResponse
                                    OrderRequest omsOrderRequest = new OrderRequest();
                                    

                                    ocrOrderResponse = OcrOrderResponse(shipmentScheduleOcrRequest);
                                    if(ocrOrderResponse != null && ocrOrderResponse.StatusCode == (int)HttpStatusCode.OK)
                                    {
                                        LoginRequest omsLoginRequest = new LoginRequest();
                                        string omsToken = "";
                                        omsLoginRequest.UserName = ConfigurationManager.AppSettings["OMSLogin"];
                                        omsLoginRequest.UserPassword = ConfigurationManager.AppSettings["OMSPassword"];
                                        var tmsLoginResponse = JsonConvert.DeserializeObject<UserResponse>(GetApiResponse(ConfigurationManager.AppSettings["ApiGatewayOMSURL"]
                                                                                                                          + "v1/user/login", Method.POST, omsLoginRequest, null));
                                        if (tmsLoginResponse != null && tmsLoginResponse.Data.Count > 0)
                                        {
                                            omsToken = tmsLoginResponse.TokenKey;
                                        }
                                        omsOrderRequest.Requests = ocrOrderResponse.Data;

                                        omsOrderResponse = JsonConvert.DeserializeObject<OrderResponse>(GetApiResponse(ConfigurationManager.AppSettings["ApiGatewayOMSURL"]
                                                                                                             + "v1/order/createordersfromshipmentlistocr", Method.POST, omsOrderRequest, omsToken));

                                        if(omsOrderResponse.Data != null && omsOrderResponse.StatusCode == (int)HttpStatusCode.OK)
                                        {
                                            var tmsToken = Request.Headers.GetValues("Token").FirstOrDefault();
                                            if(tmsToken != null)
                                            {
                                                OrderRequest tmsOrderRequest = new OrderRequest();
                                                tmsOrderRequest.Requests = omsOrderResponse.Data;
                                                tmsOrderRequest.UploadType = 3;
                                                tmsOrderResponse = JsonConvert.DeserializeObject<OrderResponse>(GetApiResponse(ConfigurationManager.AppSettings["ApiGatewayTMSURL"]
                                                                                                                                                           + "v1/order/createordersfromshipmentlistocr", Method.POST, tmsOrderRequest, tmsToken));
                                            }
                                        }

                                    }

                                    
                                }
                            }
                        }
                    }
                }

            }
            return Ok(tmsOrderResponse);
        }

        [HttpPost, Route("shipmentscheduleocr"), AllowAnonymous]
        public async Task<IHttpActionResult> ShipmentScheduleOCR()
        {
            if (!Request.Content.IsMimeMultipartContent("form-data"))
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }

            var client = new HttpClient();
            var response = await client.PostAsync(ConfigurationManager.AppSettings["ShipmentScheduleOCRURL"], Request.Content);
            var json = response.Content.ReadAsStringAsync().Result;

            return Ok(json);
        }

        [Route("createordersfromshipmentlistocr")]
        [HttpPost]
        public IHttpActionResult CreateOrdersFromShipmentListOCR(OrderRequest request)
        {
            for (int i = 0; i < request.Requests.Count; i++)
            {

                ModelState.Remove("request.Requests[" + i + "].BusinessArea");
                ModelState.Remove("request.Requests[" + i + "].ShippingListNo");
                ModelState.Remove("request.Requests[" + i + "].PackingSheetNo");
                ModelState.Remove("request.Requests[" + i + "].TotalCollie");
                ModelState.Remove("request.Requests[" + i + "].PartnerName1");
                ModelState.Remove("request.Requests[" + i + "].PartnerName2");
                ModelState.Remove("request.Requests[" + i + "].PartnerName3");
                ModelState.Remove("request.Requests[" + i + "].Dimension");
                ModelState.Remove("request.Requests[" + i + "].ShipmentSAPNo");
                ModelState.Remove("request.Requests[" + i + "].OrderNo");
                ModelState.Remove("request.Requests[" + i + "].EstimationShipmentDate");
                ModelState.Remove("request.Requests[" + i + "].EstimationShipmentTime");
                ModelState.Remove("request.Requests[" + i + "].ActualShipmentDate");
                ModelState.Remove("request.Requests[" + i + "].ActualShipmentTime");

            }
            if (!ModelState.IsValid)
            {
                ErrorResponse errorResponse = new ErrorResponse()
                {
                    Status = DomainObjects.Resource.ResourceData.Failure,
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
            OrderResponse orderData = orderTask.CreateOrdersFromShipmentListOCR(request);

           
            return Ok(orderData);
        }

        [Route("swapestoppoints")]
        [HttpPost]
        public IHttpActionResult SwapeStopPoints(OrderStatusRequest orderStatusRequest)
        {
            IOrderTask orderTask = Helper.Model.DependencyResolver.DependencyResolver.GetImplementationOf<ITaskGateway>().OrderTask;
            OrderStatusResponse orderStatusResponse = orderTask.SwapeStopPoints(orderStatusRequest);

            return Ok(orderStatusResponse);
        }

    }
}
