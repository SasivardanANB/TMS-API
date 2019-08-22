using System.Collections.Generic;
using System.Configuration;
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

namespace TMS.API.Controllers
{
    [CustomAuthorize]
    [RoutePrefix("api/v1/order")]
    public class OrderController : ApiController
    {

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

            if (Request.Headers.Contains("Token"))
            {
                order.Token = Request.Headers.GetValues("Token").First();
            }

            // Create Order in TMS
            IOrderTask tmsOrderTask = Helper.Model.DependencyResolver.DependencyResolver.GetImplementationOf<ITaskGateway>().OrderTask;
            OrderResponse tmsOrderResponse = tmsOrderTask.CreateUpdateOrder(order);

            return Ok(tmsOrderResponse);
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
            request.Token = Request.Headers.GetValues("Token").FirstOrDefault();

            IOrderTask orderTask = Helper.Model.DependencyResolver.DependencyResolver.GetImplementationOf<ITaskGateway>().OrderTask;
            OrderStatusResponse response = orderTask.UpdateOrderStatus(request);
            
            return Ok(response);
        }

        [Route("getshippinglistguids")]
        [HttpGet]
        public IHttpActionResult GetShippingListGuids(string orderNumber)
        {
            IOrderTask orderTask = Helper.Model.DependencyResolver.DependencyResolver.GetImplementationOf<ITaskGateway>().OrderTask;
            ImageGuidsResponse response = orderTask.GetShippingListGuids(orderNumber);

            return Ok(response);
        }

        [Route("getpodguids")]
        [HttpGet]
        public IHttpActionResult GetPodGuids(string orderNumber)
        {
            IOrderTask orderTask = Helper.Model.DependencyResolver.DependencyResolver.GetImplementationOf<ITaskGateway>().OrderTask;
            ImageGuidsResponse response = orderTask.GetPodGuids(orderNumber);

            return Ok(response);
        }

        [Route("getphotowithcustomerguids")]
        [HttpGet]
        public IHttpActionResult GetPhotoWithCustomerGuids(string orderNumber)
        {
            IOrderTask orderTask = Helper.Model.DependencyResolver.DependencyResolver.GetImplementationOf<ITaskGateway>().OrderTask;
            ImageGuidsResponse response = orderTask.GetPhotoWithCustomerGuids(orderNumber);

            return Ok(response);
        }

        [Route("cancelorder")]
        [HttpPost]
        public IHttpActionResult CancelOrder(OrderStatusRequest request)
        {
            IOrderTask orderTask = Helper.Model.DependencyResolver.DependencyResolver.GetImplementationOf<ITaskGateway>().OrderTask;
            OrderStatusResponse response = orderTask.CancelOrder(request);

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
            var token = Request.Headers.GetValues("Token").FirstOrDefault();

            IOrderTask orderTask = Helper.Model.DependencyResolver.DependencyResolver.GetImplementationOf<ITaskGateway>().OrderTask;
            OrderResponse tmsOrderResponse = orderTask.GetShipmentSchedulesFromEmail(token);
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

        [Route("ocrorderresponse")]
        [HttpPost,AllowAnonymous]
        public IHttpActionResult OcrOrderResponse(ShipmentScheduleOcrRequest shipmentScheduleOcrRequest)
        {
            IOrderTask orderTask = Helper.Model.DependencyResolver.DependencyResolver.GetImplementationOf<ITaskGateway>().OrderTask;
            OrderResponse orderResponse = orderTask.OcrOrderResponse(shipmentScheduleOcrRequest);
            return Ok(orderResponse);
        }
    }
}
