using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
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

        private readonly IOrder _orderRepository;
        public BusinessOrderTask(IOrder orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public override OrderResponse CreateUpdateOrder(OrderRequest order)
        {
            //If needed write business logic here for request.

            OrderResponse orderData = _orderRepository.CreateUpdateOrder(order);

            //If needed write business logic here for response.
            return orderData;
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
                var tmsLoginResponse = JsonConvert.DeserializeObject<UserResponse>(GetApiResponse(ConfigurationManager.AppSettings["ApiGatewayOMSURL"]
                    + "/v1/user/login", Method.POST, loginRequest, null));
                if (tmsLoginResponse != null && tmsLoginResponse.Data.Count > 0)
                {
                    token = tmsLoginResponse.TokenKey;
                }
                #endregion

                var omsresponse = JsonConvert.DeserializeObject<OrderStatusResponse>(GetApiResponse(ConfigurationManager.AppSettings["ApiGatewayOMSURL"]
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
    }

}
