using Newtonsoft.Json;
using NLog;
using OMS.API.Classes;
using OMS.DomainGateway.Gateway;
using OMS.DomainGateway.Gateway.Interfaces;
using OMS.DomainGateway.Task;
using OMS.DomainGateway.Task.Interfaces;
using OMS.DomainObjects.Objects;
using OMS.DomainObjects.Request;
using OMS.DomainObjects.Response;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.ModelBinding;

namespace OMS.API.Controllers
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
        #endregion

        [Route("getorders")]
        [HttpPost]
        public IHttpActionResult GetOrders(DownloadOrderRequest order)
        {

            IOrderTask orderTask = Helper.Model.DependencyResolver.DependencyResolver.GetImplementationOf<ITaskGateway>().OrderTask;
            OrderResponse orderData = orderTask.GetOrders(order);

            return Ok(orderData);
        }

        [Route("createupdateorders")]
        [HttpPost]
        public IHttpActionResult CreateUpdateOrders(OrderRequest request)
        {
            for (int i = 0; i < request.Requests.Count; i++)
            {
                if (request.Requests[i].OrderType == 1) //For Inbound
                {
                    if (request.Requests[i].OrderWeight == 0)
                    {
                        ModelState.AddModelError($"{nameof(request)}.{nameof(request.Requests)}.[{i}].{nameof(Order.OrderWeight)}", "Invalid Order Weight");
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(request.Requests[i].OrderWeightUM))
                        {
                            ModelState.AddModelError($"{nameof(request)}.{nameof(request.Requests)}.[{i}].{nameof(Order.OrderWeightUM)}", "Invalid Order Weight UM");
                        }
                    }
                    if (string.IsNullOrEmpty(request.Requests[i].ShippingListNo))
                    {
                        ModelState.AddModelError($"{nameof(request)}.{nameof(request.Requests)}.[{i}].{nameof(Order.ShippingListNo)}", "Invalid Shipping List Number");
                    }
                    if (string.IsNullOrEmpty(request.Requests[i].PackingSheetNo))
                    {
                        ModelState.AddModelError($"{nameof(request)}.{nameof(request.Requests)}.[{i}].{nameof(Order.PackingSheetNo)}", "Invalid Packing Sheet Number");
                    }
                    if (!string.IsNullOrEmpty(request.Requests[i].PackingSheetNo))
                    {
                        string[] packingSheets = request.Requests[i].PackingSheetNo.Split(',');
                        foreach (string packingSheet in packingSheets)
                        {
                            if (packingSheet.Length > 20)
                            {
                                ModelState.AddModelError($"{nameof(request)}.{nameof(request.Requests)}.[{i}].{nameof(Order.PackingSheetNo)}", "Packing Sheet Number should not exceed 20 characters");
                            }
                        }
                    }
                    if (request.Requests[i].TotalCollie == 0)
                    {
                        ModelState.AddModelError($"{nameof(request)}.{nameof(request.Requests)}.[{i}].{nameof(Order.TotalCollie)}", "Invalid Total Collie");
                    }
                }
                else if (request.Requests[i].OrderType == 2)//For Outbound
                {
                    if (string.IsNullOrEmpty(request.Requests[i].ShipmentSAPNo))
                        ModelState.AddModelError($"{nameof(request)}.{nameof(request.Requests)}.[{i}].{nameof(Order.ShipmentSAPNo)}", "Invalid Shipment SAP Number");
                }
                else
                    ModelState.AddModelError($"{nameof(request)}.{nameof(request.Requests)}.[{i}].{nameof(Order.OrderType)}", "Invalid Order Type");
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
            OrderResponse orderData = orderTask.CreateUpdateOrders(request);

            if (orderData.StatusCode == 200 && orderData.Status == "Success" && request.orderGeneratedSystem != "TMS")
            {
                #region Call TMS Order Request
                if (orderData.StatusCode == (int)HttpStatusCode.OK)
                {

                    OrderRequest tmsRequest = new OrderRequest()
                    {
                        Requests = new List<Order>(),
                        CreatedBy = "OMS System",
                        UploadType = 1,
                        orderGeneratedSystem = "OMS"
                    };

                    //Login to TMS and get Token
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
                    foreach (var omsOrder in request.Requests)
                    {
                        Order tmsOrder = new Order()
                        {
                            BusinessArea = omsOrder.BusinessArea,
                            OrderNo = omsOrder.OrderNo,
                            SequenceNo = omsOrder.SequenceNo,
                            PartnerNo1 = omsOrder.PartnerNo1,
                            PartnerType1 = omsOrder.PartnerType1,
                            PartnerName1 = omsOrder.PartnerName1,
                            PartnerNo2 = omsOrder.PartnerNo2,
                            PartnerType2 = omsOrder.PartnerType2,
                            PartnerName2 = omsOrder.PartnerName2,
                            PartnerNo3 = omsOrder.PartnerNo3,
                            PartnerType3 = omsOrder.PartnerType3,
                            PartnerName3 = omsOrder.PartnerName3,
                            FleetType = omsOrder.FleetType,
                            OrderType = omsOrder.OrderType,
                            VehicleShipmentType = omsOrder.VehicleShipmentType,
                            DriverNo = omsOrder.DriverNo,
                            DriverName = omsOrder.DriverName,
                            VehicleNo = omsOrder.VehicleNo,
                            OrderWeight = omsOrder.OrderWeight,
                            OrderWeightUM = omsOrder.OrderWeightUM,
                            EstimationShipmentDate = omsOrder.EstimationShipmentDate,
                            EstimationShipmentTime = omsOrder.EstimationShipmentTime,
                            ActualShipmentDate = omsOrder.ActualShipmentDate,
                            ActualShipmentTime = omsOrder.ActualShipmentTime,
                            Sender = omsOrder.Sender,
                            Receiver = omsOrder.Receiver,
                            OrderShipmentStatus = omsOrder.OrderShipmentStatus,
                            Dimension = omsOrder.Dimension,
                            TotalPallet = omsOrder.TotalPallet,
                            Instructions = omsOrder.Instructions,
                            ShippingListNo = omsOrder.ShippingListNo,
                            PackingSheetNo = omsOrder.PackingSheetNo,
                            TotalCollie = omsOrder.TotalCollie,
                            ShipmentSAPNo = omsOrder.ShipmentSAPNo
                        };
                        tmsRequest.Requests.Add(tmsOrder);
                    }

                    OrderResponse tmsOrderData = JsonConvert.DeserializeObject<OrderResponse>(GetApiResponse(ConfigurationManager.AppSettings["ApiGatewayTMSURL"]
                        + "/v1/order/createupdateorder", Method.POST, tmsRequest, token));
                    if (tmsOrderData.StatusCode == (int)HttpStatusCode.OK)
                    {
                        orderData.StatusMessage = orderData.StatusMessage + ". " + tmsOrderData.StatusMessage;
                    }
                }
                #endregion
            }
            return Ok(orderData);
        }

        /// <summary>
        /// This method deals manually entered orders in TMS
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Route("syncOrders")]
        [HttpPost]
        public IHttpActionResult SyncOrders(OrderRequest order)
        {
            #region Model Validation
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
                    if (order.UploadType == 2) // Create Order
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

                    if (order.UploadType == 2) // Upload Order
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
            #endregion

            //Create order in OMS
            IOrderTask orderTask = Helper.Model.DependencyResolver.DependencyResolver.GetImplementationOf<ITaskGateway>().OrderTask;
            OrderResponse orderData = orderTask.SyncOrders(order);

            return Ok(orderData);
        }

        [Route("getallorderstatus")]
        [HttpGet]
        public IHttpActionResult GetAllOrderStatus()
        {
            IOrderTask orderTask = Helper.Model.DependencyResolver.DependencyResolver.GetImplementationOf<ITaskGateway>().OrderTask;
            OrderStatusCodesResponse orderStatusData = orderTask.GetAllOrderStatus();

            return Ok(orderStatusData);

        }

        [Route("createupdatepackingsheet")]
        [HttpPost]
        public IHttpActionResult CreateUpdatePackingSheet(PackingSheetRequest packingSheetRequest)
        {
            IOrderTask orderTask = Helper.Model.DependencyResolver.DependencyResolver.GetImplementationOf<ITaskGateway>().OrderTask;
            PackingSheetResponse packingSheetResponse = orderTask.CreateUpdatePackingSheet(packingSheetRequest);
            return Ok(packingSheetResponse);
        }

        [Route("updateorderstatus")]
        [HttpPost]
        public IHttpActionResult UpdateOrderStatus(OrderStatusRequest request)
        {
            IOrderTask orderTask = Helper.Model.DependencyResolver.DependencyResolver.GetImplementationOf<ITaskGateway>().OrderTask;
            OrderStatusResponse response = orderTask.UpdateOrderStatus(request);
            return Ok(response);
        }
        [Route("reassigntrip")]
        [HttpPost]
        public IHttpActionResult ReAssignTrip(TripRequest tripRequest)
        {
            IOrderTask orderTask = Helper.Model.DependencyResolver.DependencyResolver.GetImplementationOf<ITaskGateway>().OrderTask;
            TripResponse tripData = orderTask.ReAssignTrip(tripRequest);
            return Ok(tripData);
        }

        [Route("cancelorder")]
        [HttpPost]
        public IHttpActionResult CancelOrder(OrderStatusRequest request)
        {
            IOrderTask orderTask = Helper.Model.DependencyResolver.DependencyResolver.GetImplementationOf<ITaskGateway>().OrderTask;
            OrderStatusResponse response = orderTask.CancelOrder(request);
            return Ok(response);
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

                ModelState.Remove("request.Requests[" + i + "].OrderNo ");
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

            //if (orderData.StatusCode == 200 && orderData.Status == "Success" && request.orderGeneratedSystem != "TMS")
            //{
            //    #region Call TMS Order Request
            //    if (orderData.StatusCode == (int)HttpStatusCode.OK)
            //    {

            //        OrderRequest tmsRequest = new OrderRequest()
            //        {
            //            Requests = new List<Order>(),
            //            CreatedBy = "OMS System",
            //            UploadType = 1,
            //            orderGeneratedSystem = "OMS"
            //        };

            //        //Login to TMS and get Token
            //        LoginRequest loginRequest = new LoginRequest();
            //        string token = "";
            //        loginRequest.UserName = ConfigurationManager.AppSettings["TMSLogin"];
            //        loginRequest.UserPassword = ConfigurationManager.AppSettings["TMSPassword"];
            //        var tmsLoginResponse = JsonConvert.DeserializeObject<UserResponse>(GetApiResponse(ConfigurationManager.AppSettings["ApiGatewayTMSURL"]
            //            + "/v1/user/login", Method.POST, loginRequest, null));
            //        if (tmsLoginResponse != null && tmsLoginResponse.Data.Count > 0)
            //        {
            //            token = tmsLoginResponse.TokenKey;
            //        }
            //        foreach (var omsOrder in request.Requests)
            //        {
            //            Order tmsOrder = new Order()
            //            {
            //                BusinessArea = omsOrder.BusinessArea,
            //                OrderNo = omsOrder.OrderNo,
            //                SequenceNo = omsOrder.SequenceNo,
            //                PartnerNo1 = omsOrder.PartnerNo1,
            //                PartnerType1 = omsOrder.PartnerType1,
            //                PartnerName1 = omsOrder.PartnerName1,
            //                PartnerNo2 = omsOrder.PartnerNo2,
            //                PartnerType2 = omsOrder.PartnerType2,
            //                PartnerName2 = omsOrder.PartnerName2,
            //                PartnerNo3 = omsOrder.PartnerNo3,
            //                PartnerType3 = omsOrder.PartnerType3,
            //                PartnerName3 = omsOrder.PartnerName3,
            //                FleetType = omsOrder.FleetType,
            //                OrderType = omsOrder.OrderType,
            //                VehicleShipmentType = omsOrder.VehicleShipmentType,
            //                DriverNo = omsOrder.DriverNo,
            //                DriverName = omsOrder.DriverName,
            //                VehicleNo = omsOrder.VehicleNo,
            //                OrderWeight = omsOrder.OrderWeight,
            //                OrderWeightUM = omsOrder.OrderWeightUM,
            //                EstimationShipmentDate = omsOrder.EstimationShipmentDate,
            //                EstimationShipmentTime = omsOrder.EstimationShipmentTime,
            //                ActualShipmentDate = omsOrder.ActualShipmentDate,
            //                ActualShipmentTime = omsOrder.ActualShipmentTime,
            //                Sender = omsOrder.Sender,
            //                Receiver = omsOrder.Receiver,
            //                OrderShipmentStatus = omsOrder.OrderShipmentStatus,
            //                Dimension = omsOrder.Dimension,
            //                TotalPallet = omsOrder.TotalPallet,
            //                Instructions = omsOrder.Instructions,
            //                ShippingListNo = omsOrder.ShippingListNo,
            //                PackingSheetNo = omsOrder.PackingSheetNo,
            //                TotalCollie = omsOrder.TotalCollie,
            //                ShipmentSAPNo = omsOrder.ShipmentSAPNo
            //            };
            //            tmsRequest.Requests.Add(tmsOrder);
            //        }

            //        OrderResponse tmsOrderData = JsonConvert.DeserializeObject<OrderResponse>(GetApiResponse(ConfigurationManager.AppSettings["ApiGatewayTMSURL"]
            //            + "/v1/order/createupdateorder", Method.POST, tmsRequest, token));
            //        if (tmsOrderData.StatusCode == (int)HttpStatusCode.OK)
            //        {
            //            orderData.StatusMessage = orderData.StatusMessage + ". " + tmsOrderData.StatusMessage;
            //        }
            //    }
            //    #endregion
            //}
            return Ok(orderData);
        }

    }
}
