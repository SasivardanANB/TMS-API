using ActiveUp.Net.Mail;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using TMS.BusinessGateway.Classes;
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

        public override OrderResponse CreateUpdateOrder(OrderRequest order)
        {
            OrderResponse tmsOrderResponse = new OrderResponse();
            OrderResponse omsOrderResponse = new OrderResponse();

            //Login to OMS and get Token
            LoginRequest omsLoginRequest = new LoginRequest();
            string omsToken = "";
            omsLoginRequest.UserName = ConfigurationManager.AppSettings["OMSLogin"];
            omsLoginRequest.UserPassword = ConfigurationManager.AppSettings["OMSPassword"];
            var tmsLoginResponse = JsonConvert.DeserializeObject<UserResponse>(Utility.GetApiResponse(ConfigurationManager.AppSettings["ApiGatewayOMSURL"]
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



                        #region check driverdetails availability and if exist set status as assigned(3) else Created (1)  
                        int OrderShipmentStatus = tmsOrder.OrderShipmentStatus;
                        if (!String.IsNullOrEmpty(tmsOrder.DriverNo) && !String.IsNullOrEmpty(tmsOrder.DriverName))
                        {
                            OrderShipmentStatus = 3;
                        }

                        #endregion

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
                            OrderShipmentStatus = OrderShipmentStatus,
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
                    omsOrderResponse = JsonConvert.DeserializeObject<OrderResponse>(Utility.GetApiResponse(ConfigurationManager.AppSettings["ApiGatewayOMSURL"]
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

                        Dictionary<string, List<string>> destPartnerEmails = new Dictionary<string, List<string>>();

                        foreach (var request in order.Requests)
                        {
                            Partner destinationPartnerDetail = GetPartnerDetail(request.PartnerNo3, order.UploadType);

                            if (destPartnerEmails.ContainsKey(request.OrderNo) )
                            {
                                if (request.OrderType == 2) // outbound
                                {
                                    destPartnerEmails.TryGetValue(request.OrderNo, out List<string> lstEmails);
                                    if (lstEmails != null && !lstEmails.Contains(destinationPartnerDetail.PartnerEmail))
                                    {
                                        lstEmails.Add(destinationPartnerDetail.PartnerEmail);
                                    }
                                    destPartnerEmails[request.OrderNo] = lstEmails;
                                }
                            }
                            else
                            {
                                List<string> lstEmails = new List<string>
                                {
                                    destinationPartnerDetail.PartnerEmail
                                };
                                destPartnerEmails.Add(request.OrderNo, lstEmails);
                            }

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

                        try
                        {
                            foreach (string ord in destPartnerEmails.Keys)
                            {
                                MailMessage mail = new MailMessage();
                                mail.To.Add(string.Join(",", destPartnerEmails[ord]));
                                string emailFrom = ConfigurationManager.AppSettings["EmailFrom"];
                                mail.From = new MailAddress(emailFrom);
                                mail.Subject = "Your Order Delivery Status";
                                string Body = "Dear Customer, <br /> This is your order deliver status. Your order delivery No is " + ord + ". You can track order by clicking this link " + ConfigurationManager.AppSettings["TMS_APP_URL"];
                                mail.Body = Body;
                                mail.IsBodyHtml = true;
                                string smtpHost = ConfigurationManager.AppSettings["SmtpHost"];
                                string loginEmailId = ConfigurationManager.AppSettings["SmtpUserName"];
                                string emailPassword = ConfigurationManager.AppSettings["SmtpPassword"];
                                System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient(smtpHost)
                                {
                                    Port = Convert.ToInt32(ConfigurationManager.AppSettings["smtpPort"]),
                                    UseDefaultCredentials = false,
                                    DeliveryMethod = SmtpDeliveryMethod.Network,
                                    Credentials = new System.Net.NetworkCredential(loginEmailId, emailPassword),
                                    EnableSsl = true
                                };
                                smtp.Send(mail);
                            }
                        }
                        catch(Exception ex)
                        {
                            // Continue execution
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
                            var dmsLoginResponse = JsonConvert.DeserializeObject<UserResponse>(Utility.GetApiResponse(ConfigurationManager.AppSettings["ApiGatewayDMSURL"]
                                + "/v1/user/login", Method.POST, loginRequest, null));
                            if (dmsLoginResponse != null && dmsLoginResponse.Data.Count > 0)
                            {
                                token = dmsLoginResponse.TokenKey;
                            }
                            #endregion

                            var response = JsonConvert.DeserializeObject<UserResponse>(Utility.GetApiResponse(ConfigurationManager.AppSettings["ApiGatewayDMSURL"]
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
                List<string> destPartnerEmails = new List<string>();
                string orderNumber = string.Empty;

                DriverRequest driverRequest = new DriverRequest();
                driverRequest.Requests = new List<Driver>();
                driverRequest.Requests.Add(new Driver
                {
                    DriverNo = order.Requests[0].DriverNo
                });

                var driverData = JsonConvert.DeserializeObject<DriverResponse>(Utility.GetApiResponse(ConfigurationManager.AppSettings["ApiGatewayTMSURL"]
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

                    if (!destPartnerEmails.Contains(partner2Data.PartnerEmail))
                    {
                        destPartnerEmails.Add(partner3Data.PartnerEmail);
                    }

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

                omsOrderResponse = JsonConvert.DeserializeObject<OrderResponse>(Utility.GetApiResponse(ConfigurationManager.AppSettings["ApiGatewayOMSURL"]
                                                                                                          + "v1/order/syncorders", Method.POST, omsRequest, omsToken));

                if (omsOrderResponse.StatusCode == (int)HttpStatusCode.OK)
                {
                    // Get orderNumber from oms response and update tmsRequest
                    foreach (var ord in order.Requests)
                    {
                        ord.OrderNo = omsOrderResponse.Data[0].OrderNo;
                        orderNumber = omsOrderResponse.Data[0].OrderNo;
                        ord.LegecyOrderNo = omsOrderResponse.Data[0].OrderNo;
                        int OrderShipmentStatus = ord.OrderShipmentStatus;
                        if (ord.DriverNo != null && ord.DriverName != null && ord.DriverNo != string.Empty && ord.DriverName != string.Empty)
                        {
                            OrderShipmentStatus = 3;
                        }
                        ord.OrderShipmentStatus = OrderShipmentStatus;

                    }

                    // Create Order in TMS
                    tmsOrderResponse = _orderRepository.CreateUpdateOrder(order);

                    omsOrderResponse.StatusMessage = omsOrderResponse.StatusMessage + ". " + tmsOrderResponse.StatusMessage;

                    if (tmsOrderResponse.StatusCode == 200 && tmsOrderResponse.Status == "Success")
                    {
                        #region send email to destination partners
                        if (destPartnerEmails.Count > 0 && !String.IsNullOrEmpty(orderNumber))
                        {
                            MailMessage mail = new MailMessage();
                            mail.To.Add(string.Join(",", destPartnerEmails));
                            string emailFrom = ConfigurationManager.AppSettings["EmailFrom"];
                            mail.From = new MailAddress(emailFrom);
                            mail.Subject = "Your Order Delivery Status";
                            string Body = "Dear Customer, <br /> This is your order deliver status. Your order delivery No is " + orderNumber + ". You can track order by clicking this link " + ConfigurationManager.AppSettings["TMS_APP_URL"];
                            mail.Body = Body;
                            mail.IsBodyHtml = true;
                            string smtpHost = ConfigurationManager.AppSettings["SmtpHost"];
                            string loginEmailId = ConfigurationManager.AppSettings["SmtpUserName"];
                            string emailPassword = ConfigurationManager.AppSettings["SmtpPassword"];
                            System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient(smtpHost)
                            {
                                Port = Convert.ToInt32(ConfigurationManager.AppSettings["smtpPort"]),
                                UseDefaultCredentials = false,
                                DeliveryMethod = SmtpDeliveryMethod.Network,
                                Credentials = new System.Net.NetworkCredential(loginEmailId, emailPassword),
                                EnableSsl = true
                            };
                            smtp.Send(mail);
                        }
                        #endregion

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
                            var dmsLoginResponse = JsonConvert.DeserializeObject<UserResponse>(Utility.GetApiResponse(ConfigurationManager.AppSettings["ApiGatewayDMSURL"]
                                + "/v1/user/login", Method.POST, loginRequest, null));
                            if (dmsLoginResponse != null && dmsLoginResponse.Data.Count > 0)
                            {
                                token = dmsLoginResponse.TokenKey;
                            }
                            #endregion

                            var response = JsonConvert.DeserializeObject<UserResponse>(Utility.GetApiResponse(ConfigurationManager.AppSettings["ApiGatewayDMSURL"]
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
            PackingSheetResponse packingSheetResponse = _orderRepository.CreateUpdatePackingSheet(packingSheetRequest);

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
                var tmsLoginResponse = JsonConvert.DeserializeObject<UserResponse>(Utility.GetApiResponse(ConfigurationManager.AppSettings["ApiGatewayOMSURL"]
                                                                                                  + "v1/user/login", Method.POST, omsLoginRequest, null));
                if (tmsLoginResponse != null && tmsLoginResponse.Data.Count > 0)
                {
                    omsToken = tmsLoginResponse.TokenKey;
                }

                PackingSheetResponse omsPackingSheetResponse = JsonConvert.DeserializeObject<PackingSheetResponse>(Utility.GetApiResponse(ConfigurationManager.AppSettings["ApiGatewayOMSURL"]
                                                                                                          + "v1/order/CreateUpdatePackingSheet", Method.POST, packingSheetRequest, omsToken));

                packingSheetResponse.StatusMessage = omsPackingSheetResponse.StatusMessage + " " + packingSheetResponse.StatusMessage;

                #endregion
            }

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

            CommonResponse commonResponse = _orderRepository.GetOrderIds(tokenValue);

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
            OrderStatusResponse response = _orderRepository.UpdateOrderStatus(request);

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
                var tmsLoginResponse = JsonConvert.DeserializeObject<UserResponse>(Utility.GetApiResponse(ConfigurationManager.AppSettings["ApiGatewayOMSURL"]
                    + "/v1/user/login", Method.POST, loginRequest, null));
                if (tmsLoginResponse != null && tmsLoginResponse.Data.Count > 0)
                {
                    token = tmsLoginResponse.TokenKey;
                }
                #endregion

                var omsresponse = JsonConvert.DeserializeObject<UserResponse>(Utility.GetApiResponse(ConfigurationManager.AppSettings["ApiGatewayOMSURL"]
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
                InvoiceResponse invoiceResponse = GetInvoiceRequest(request);
                if (invoiceResponse != null && invoiceResponse.Data.Count > 0)
                {
                    InvoiceRequest invoiceRequest = new InvoiceRequest();
                    invoiceRequest.Requests = invoiceResponse.Data;
                    if (!String.IsNullOrEmpty(request.Token))
                    {
                        JsonConvert.DeserializeObject<InvoiceResponse>(Utility.GetApiResponse(ConfigurationManager.AppSettings["ApiGatewayTMSURL"] + "v1/invoice/generateinvoice", Method.POST, invoiceRequest, request.Token));
                    }
                }
            }
            #endregion

            return response;
        }

        public override OrderStatusResponse CancelOrder(OrderStatusRequest request)
        {
            OrderStatusResponse response = _orderRepository.CancelOrder(request);

            if (response.StatusCode == (int)HttpStatusCode.OK && response.Status == DomainObjects.Resource.ResourceData.Success)
            {
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
                    var tmsLoginResponse = JsonConvert.DeserializeObject<UserResponse>(Utility.GetApiResponse(ConfigurationManager.AppSettings["ApiGatewayOMSURL"]
                        + "/v1/user/login", Method.POST, loginRequest, null));
                    if (tmsLoginResponse != null && tmsLoginResponse.Data.Count > 0)
                    {
                        token = tmsLoginResponse.TokenKey;
                    }
                    #endregion

                    var omsresponse = JsonConvert.DeserializeObject<UserResponse>(Utility.GetApiResponse(ConfigurationManager.AppSettings["ApiGatewayOMSURL"]
                        + "/v1/order/cancelorder", Method.POST, omsRequest, token));
                    response.StatusMessage += ". " + omsresponse.StatusMessage;
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
                    var dmsLoginResponse = JsonConvert.DeserializeObject<UserResponse>(Utility.GetApiResponse(ConfigurationManager.AppSettings["ApiGatewayDMSURL"]
                        + "/v1/user/login", Method.POST, dmsLoginRequest, null));
                    if (dmsLoginResponse != null && dmsLoginResponse.Data.Count > 0)
                    {
                        dmsToken = dmsLoginResponse.TokenKey;
                    }
                    #endregion

                    var dmsresponse = JsonConvert.DeserializeObject<UserResponse>(Utility.GetApiResponse(ConfigurationManager.AppSettings["ApiGatewayDMSURL"]
                        + "/v1/trip/cancelorder", Method.POST, omsRequest, dmsToken));
                    response.StatusMessage += ". " + dmsresponse.StatusMessage;
                    #endregion
                }
                #endregion
            }
            return response;
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

        public override OrderStatusResponse SwapeStopPoints(OrderStatusRequest orderStatusRequest)
        {
            OrderStatusResponse orderStatusResponse = _orderRepository.SwapeStopPoints(orderStatusRequest);

            #region Update Status to OMS 
            OrderStatusRequest omsRequest = new OrderStatusRequest()
            {
                Requests = new List<OrderStatus>()
            };

            foreach (var item in orderStatusRequest.Requests)
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
                var tmsLoginResponse = JsonConvert.DeserializeObject<UserResponse>(Utility.GetApiResponse(ConfigurationManager.AppSettings["ApiGatewayOMSURL"]
                    + "/v1/user/login", Method.POST, loginRequest, null));
                if (tmsLoginResponse != null && tmsLoginResponse.Data.Count > 0)
                {
                    token = tmsLoginResponse.TokenKey;
                }
                #endregion

                var omsresponse = JsonConvert.DeserializeObject<OrderStatusResponse>(Utility.GetApiResponse(ConfigurationManager.AppSettings["ApiGatewayOMSURL"]
                    + "/v1/order/swapestoppoints", Method.POST, omsRequest, token));
                if (omsresponse != null)
                {
                    orderStatusResponse.StatusMessage += ". " + omsresponse.StatusMessage;
                }
                #endregion
            }
            #endregion


            return orderStatusResponse;
        }

        public override ImageGuidsResponse GetShippingListGuids(string orderNumber)
        {
            #region Login to DMS and get Token
            LoginRequest loginRequest = new LoginRequest();
            string token = "";

            loginRequest.UserName = ConfigurationManager.AppSettings["DMSLogin"];
            loginRequest.UserPassword = ConfigurationManager.AppSettings["DMSPassword"];
            var dmsLoginResponse = JsonConvert.DeserializeObject<UserResponse>(Utility.GetApiResponse(ConfigurationManager.AppSettings["ApiGatewayDMSURL"]
                + "/v1/user/login", Method.POST, loginRequest, null));
            if (dmsLoginResponse != null && dmsLoginResponse.Data.Count > 0)
            {
                token = dmsLoginResponse.TokenKey;
            }
            #endregion

            ImageGuidsResponse response = JsonConvert.DeserializeObject<ImageGuidsResponse>(Utility.GetApiResponse(ConfigurationManager.AppSettings["ApiGatewayDMSURL"]
                + "/v1/trip/getshippinglistguids?orderNumber=" + orderNumber, Method.GET, null, token));

            return response;
        }

        public override ImageGuidsResponse GetPodGuids(string orderNumber)
        {
            #region Login to DMS and get Token
            LoginRequest loginRequest = new LoginRequest();
            string token = "";

            loginRequest.UserName = ConfigurationManager.AppSettings["DMSLogin"];
            loginRequest.UserPassword = ConfigurationManager.AppSettings["DMSPassword"];
            var dmsLoginResponse = JsonConvert.DeserializeObject<UserResponse>(Utility.GetApiResponse(ConfigurationManager.AppSettings["ApiGatewayDMSURL"]
                + "/v1/user/login", Method.POST, loginRequest, null));
            if (dmsLoginResponse != null && dmsLoginResponse.Data.Count > 0)
            {
                token = dmsLoginResponse.TokenKey;
            }
            #endregion

            ImageGuidsResponse response = JsonConvert.DeserializeObject<ImageGuidsResponse>(Utility.GetApiResponse(ConfigurationManager.AppSettings["ApiGatewayDMSURL"]
                + "/v1/trip/getpodguids?orderNumber=" + orderNumber, Method.GET, null, token));

            return response;
        }

        public override ImageGuidsResponse GetPhotoWithCustomerGuids(string orderNumber)
        {
            #region Login to DMS and get Token
            LoginRequest loginRequest = new LoginRequest();
            string token = "";

            loginRequest.UserName = ConfigurationManager.AppSettings["DMSLogin"];
            loginRequest.UserPassword = ConfigurationManager.AppSettings["DMSPassword"];
            var dmsLoginResponse = JsonConvert.DeserializeObject<UserResponse>(Utility.GetApiResponse(ConfigurationManager.AppSettings["ApiGatewayDMSURL"]
                + "/v1/user/login", Method.POST, loginRequest, null));
            if (dmsLoginResponse != null && dmsLoginResponse.Data.Count > 0)
            {
                token = dmsLoginResponse.TokenKey;
            }
            #endregion

            ImageGuidsResponse response = JsonConvert.DeserializeObject<ImageGuidsResponse>(Utility.GetApiResponse(ConfigurationManager.AppSettings["ApiGatewayDMSURL"]
                + "/v1/trip/getphotowithcustomerguids?orderNumber=" + orderNumber, Method.GET, null, token));

            return response;
        }

        public override OrderResponse GetShipmentSchedulesFromEmail(string token)
        {
            OrderResponse tmsOrderResponse = new OrderResponse();

            // Loin into Gamil and Get all Mails
            var mailRepository = new MailRepository(
                                    ConfigurationManager.AppSettings["ImapHost"],
                                    Convert.ToInt32(ConfigurationManager.AppSettings["ImapPort"].ToString()),
                                    true,
                                    ConfigurationManager.AppSettings["ImapUserName"],
                                    ConfigurationManager.AppSettings["ImapUserPassword"]
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
                            var fileuploadresponse = JsonConvert.DeserializeObject<ResponseDataForFileUpload>(Utility.GetFileUploadApiResponse(ConfigurationManager.AppSettings["ApiGatewayTMSURL"] + "/v1/media/uploadfile", Method.POST, attachment, null));

                            if (fileuploadresponse.StatusCode == (int)HttpStatusCode.OK && !String.IsNullOrEmpty(fileuploadresponse.Guid))
                            {
                                // Calling OCR to get shipmentschedule data
                                var res = Utility.GetFileUploadApiResponse(ConfigurationManager.AppSettings["ApiGatewayTMSURL"] + "/v1/order/shipmentscheduleocr", Method.POST, attachment, null);
                                var shipmentScheduleOcr = JsonConvert.DeserializeObject<dynamic>(res);
                                var jsonObject = JObject.Parse(shipmentScheduleOcr);
                                ShipmentScheduleOcrRequest shipmentScheduleOcrRequest = new ShipmentScheduleOcrRequest();
                                if (jsonObject.success)
                                {
                                    shipmentScheduleOcrRequest.Requests = new List<ShipmentScheduleOcr>();
                                    ShipmentScheduleOcr shipmentSchedule = new ShipmentScheduleOcr()
                                    {
                                        ImageGUID = fileuploadresponse.Guid,
                                        DocumentType = jsonObject.documentType,
                                        Success = jsonObject.success,
                                        Image = jsonObject.image,
                                        EmailFrom = email.From.Email,
                                        EmailDateTime = email.Date,
                                        EmailSubject = email.Subject,
                                        EmailText = email.BodyHtml.TextStripped,
                                        Data = new DetailsOcr()
                                        {
                                            DayShipment = jsonObject.data.DayShipment,
                                            EstimatedTotalPallet = jsonObject.data.EstimatedTotalPallet,
                                            MainDealerCode = jsonObject.data.MainDealerCode,
                                            MainDealerName = jsonObject.data.MainDealerName,
                                            MultiDropShipment = jsonObject.data.MultiDropShipment,
                                            ShipmentScheduleNo = jsonObject.data.ShipmentScheduleNo,
                                            ShipmentTime = jsonObject.data.ShipmentTime,
                                            ShipToParty = jsonObject.data.ShipToParty,
                                            VehicleType = jsonObject.data.VehicleType,
                                            Weight = jsonObject.data.Weight
                                        }
                                    };
                                    shipmentScheduleOcrRequest.Requests.Add(shipmentSchedule);
                                }

                                if (shipmentScheduleOcrRequest.Requests.Count > 0)
                                {
                                    //OcrOrderResponse
                                    OrderRequest omsOrderRequest = new OrderRequest();
                                    OrderResponse ocrOrderResponse = OcrOrderResponse(shipmentScheduleOcrRequest);
                                    if (ocrOrderResponse != null && ocrOrderResponse.StatusCode == (int)HttpStatusCode.OK)
                                    {
                                        LoginRequest omsLoginRequest = new LoginRequest();
                                        string omsToken = "";
                                        omsLoginRequest.UserName = ConfigurationManager.AppSettings["OMSLogin"];
                                        omsLoginRequest.UserPassword = ConfigurationManager.AppSettings["OMSPassword"];
                                        var tmsLoginResponse = JsonConvert.DeserializeObject<UserResponse>(Utility.GetApiResponse(ConfigurationManager.AppSettings["ApiGatewayOMSURL"]
                                                                                                                          + "v1/user/login", Method.POST, omsLoginRequest, null));
                                        if (tmsLoginResponse != null && tmsLoginResponse.Data.Count > 0)
                                        {
                                            omsToken = tmsLoginResponse.TokenKey;
                                        }
                                        omsOrderRequest.Requests = ocrOrderResponse.Data;

                                        OrderResponse omsOrderResponse = JsonConvert.DeserializeObject<OrderResponse>(Utility.GetApiResponse(ConfigurationManager.AppSettings["ApiGatewayOMSURL"]
                                                                                                             + "v1/order/createordersfromshipmentlistocr", Method.POST, omsOrderRequest, omsToken));

                                        if (omsOrderResponse.Data != null && omsOrderResponse.StatusCode == (int)HttpStatusCode.OK)
                                        {
                                            if (!String.IsNullOrEmpty(token))
                                            {
                                                OrderRequest tmsOrderRequest = new OrderRequest();
                                                tmsOrderRequest.Requests = omsOrderResponse.Data;
                                                tmsOrderRequest.UploadType = 3;
                                                tmsOrderResponse = JsonConvert.DeserializeObject<OrderResponse>(Utility.GetApiResponse(ConfigurationManager.AppSettings["ApiGatewayTMSURL"]
                                                                                                                                                           + "v1/order/createordersfromshipmentlistocr", Method.POST, tmsOrderRequest, token));
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return tmsOrderResponse;
        }
    }
}
