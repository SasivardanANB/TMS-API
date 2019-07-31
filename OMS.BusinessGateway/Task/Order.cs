using OMS.DataGateway.Repositories;
using OMS.DataGateway.Repositories.Iterfaces;
using OMS.DomainGateway.Task;
using Domain = OMS.DomainObjects.Objects;
using Resource = OMS.DomainObjects.Resource;
using OMS.DomainObjects.Request;
using OMS.DomainObjects.Response;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Configuration;
using Newtonsoft.Json;
using RestSharp;

namespace OMS.BusinessGateway.Task
{
    public partial class BusinessOrderTask : OrderTask
    {
        private readonly IOrder _orderRepository;

        public BusinessOrderTask(IOrder orderRepository)
        {
            _orderRepository = orderRepository;
        }

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

        public override OrderResponse GetOrders(DownloadOrderRequest order)
        {
            //If needed write business logic here for request.

            OrderResponse orderData = _orderRepository.GetOrders(order);

            //If needed write business logic here for response.
            return orderData;
        }

        public override OrderResponse CreateUpdateOrders(OrderRequest request)
        {
            OrderResponse orderData = _orderRepository.CreateUpdateOrders(request);

            if (orderData.StatusCode == (int)HttpStatusCode.OK && orderData.Status == Resource.ResourceData.Success && request.orderGeneratedSystem != "TMS")
            {
                #region Call TMS Order Request

                OrderRequest tmsRequest = new OrderRequest()
                {
                    Requests = new List<Domain.Order>(),
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
                    Domain.Order tmsOrder = new Domain.Order()
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

                #endregion
            }

            return orderData;
        }

        public override OrderResponse CreateOrdersFromShipmentListOCR(OrderRequest request)
        {
            OrderResponse orderData = _orderRepository.CreateOrdersFromShipmentListOCR(request);
            return orderData;
        }

        public override OrderStatusCodesResponse GetAllOrderStatus()
        {
            OrderStatusCodesResponse orderStatusData = _orderRepository.GetAllOrderStatus();
            return orderStatusData;
        }

        public override OrderResponse SyncOrders(OrderRequest request)
        {
            OrderResponse orderData = _orderRepository.SyncOrders(request);
            return orderData;
        }

        public override PackingSheetResponse CreateUpdatePackingSheet(PackingSheetRequest packingSheetRequest)
        {
            PackingSheetResponse packingSheetResponse = _orderRepository.CreateUpdatePackingSheet(packingSheetRequest);
            return packingSheetResponse;
        }

        public override OrderStatusResponse UpdateOrderStatus(OrderStatusRequest request)
        {
            return _orderRepository.UpdateOrderStatus(request);
        }

        public override TripResponse ReAssignTrip(TripRequest tripRequest)
        {
            return _orderRepository.ReAssignTrip(tripRequest);
        }

        public override OrderStatusResponse CancelOrder(OrderStatusRequest request)
        {
            return _orderRepository.CancelOrder(request);
        }

        public override OrderStatusResponse SwapeStopPoints(OrderStatusRequest orderStatusRequest)
        {
            return _orderRepository.SwapeStopPoints(orderStatusRequest);
        }
    }
}
