using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using TMS.DataGateway.Repositories.Interfaces;
using TMS.DomainGateway.Task;
using TMS.DomainObjects.Objects;
using TMS.DomainObjects.Request;
using TMS.DomainObjects.Response;

namespace TMS.BusinessGateway.Task
{
    public partial class BusinessOrderTask : OrderTask
    {
        private readonly IOrder _orderRepository;

        public BusinessOrderTask(IOrder orderRepository)
        {
            _orderRepository = orderRepository;
        }

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

        public override OrderResponse CreateUpdateOrder(OrderRequest order)
        {
            OrderResponse tmsOrderResponse = new OrderResponse();
            OrderResponse omsOrderResponse = new OrderResponse();

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

            if (order.UploadType == 1) // Excel
            {
                omsRequest.UploadType = 1;

                if (order.orderGeneratedSystem != "OMS") // TMS
                {
                    // Create Order in OMS
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
                    tmsOrderResponse = _orderRepository.CreateUpdateOrder(order);

                    if (order.orderGeneratedSystem != "OMS")
                    {
                        omsOrderResponse.StatusMessage = omsOrderResponse.StatusMessage + ". " + tmsOrderResponse.StatusMessage;
                    }

                    if (tmsOrderResponse.StatusCode == (int)HttpStatusCode.OK && tmsOrderResponse.Status == "Success")
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

                var driverData = JsonConvert.DeserializeObject<DriverResponse>(GetApiResponse(ConfigurationManager.AppSettings["ApiGatewayTMSURL"]
                                                                                                          + "v1/driver/getdrivers", Method.POST, driverRequest, order.Token));
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
                    tmsOrderResponse = _orderRepository.CreateUpdateOrder(order);

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
                return omsOrderResponse;
            }
            else
            {
                return tmsOrderResponse;
            }

            ////If needed write business logic here for response.
            //return orderData;
        }

        public override OrderSearchResponse GetOrders(OrderSearchRequest orderSearchRequest)
        {
            //If needed write business logic here for request.

            OrderSearchResponse orderSearchResponse = _orderRepository.GetOrders(orderSearchRequest);

            //If needed write business logic here for response.
            return orderSearchResponse;
        }

        public override PackingSheetResponse CreateUpdatePackingSheet(PackingSheetRequest packingSheetRequest)
        {
            PackingSheetResponse  packingSheetResponse = _orderRepository.CreateUpdatePackingSheet(packingSheetRequest);
            return packingSheetResponse;
        }

        public override PackingSheetResponse GetPackingSheetDetails(int orderId)
        {
            PackingSheetResponse packingSheetResponse = _orderRepository.GetPackingSheetDetails(orderId);
            return packingSheetResponse;
        }

        public override OrderTrackResponse TrackOrder(int orderId)
        {
            //If needed write business logic here for request.

            OrderTrackResponse orderTrackResponse = _orderRepository.TrackOrder(orderId);

            //If needed write business logic here for response.
            return orderTrackResponse;
        }

        public override CommonResponse GetOrderIds(string tokenValue)
        {
            //If needed write business logic here for request.

            CommonResponse commonResponse  = _orderRepository.GetOrderIds(tokenValue);

            //If needed write business logic here for response.
            return commonResponse;
        }

        public override DealerDetailsResponse GetDealers(int orderId, string searchText)
        {
            DealerDetailsResponse dealerDetailsResponse = _orderRepository.GetDealers(orderId, searchText);
            return dealerDetailsResponse;
        }

        public override OrderDetailsResponse GetOrderDetails(int orderId)
        {
            OrderDetailsResponse orderDetailsResponse = _orderRepository.GetOrderDetails(orderId);
            return orderDetailsResponse;
        }

        public override Partner GetPartnerDetail(string partnerNo, int uploadType)
        {
            return _orderRepository.GetPartnerDetail(partnerNo, uploadType);
        }

        public override string GetBusinessAreaCode(int businessAreaId)
        {
            return _orderRepository.GetBusinessAreaCode(businessAreaId);
        }

        public override OrderStatusResponse UpdateOrderStatus(OrderStatusRequest request)
        {
            return _orderRepository.UpdateOrderStatus(request);
        }

        public override OrderStatusResponse CancelOrder(OrderStatusRequest request)
        {
            return _orderRepository.CancelOrder(request);
        }

        public override HargaResponse GetHarga(HargaRequest request)
        {
            return _orderRepository.GetHarga(request);
        }

        public override ShipmentScheduleOcrResponse CreateOrderFromShipmentScheduleOcr(ShipmentScheduleOcrRequest request)
        {
            return _orderRepository.CreateOrderFromShipmentScheduleOcr(request);
        }

        public override OrderResponse OcrOrderResponse(ShipmentScheduleOcrRequest shipmentScheduleOcrRequest)
        {
            return _orderRepository.OcrOrderResponse(shipmentScheduleOcrRequest);
        }

        public override OrderResponse CreateOrdersFromShipmentListOCR(OrderRequest request)
        {
            return _orderRepository.CreateOrdersFromShipmentListOCR(request);
        }

        public override InvoiceResponse GetInvoiceRequest(OrderStatusRequest request)
        {
            return _orderRepository.GetInvoiceRequest(request);
        }
    }

}
