using NLog;
using System;
using System.Collections.Generic;
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

                        if (string.IsNullOrEmpty(order.Requests[i].ShippingListNo))
                        {
                            ModelState.AddModelError($"{nameof(order)}.{nameof(order.Requests)}.[{i}].{nameof(Order.ShippingListNo)}", "Invalid Shipping List Number");
                        }
                        if (string.IsNullOrEmpty(order.Requests[i].PackingSheetNo))
                        {
                            ModelState.AddModelError($"{nameof(order)}.{nameof(order.Requests)}.[{i}].{nameof(Order.PackingSheetNo)}", "Invalid Packing Sheet Number");
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
                    }
                    else
                    {
                        ModelState.AddModelError($"{nameof(order)}.{nameof(order.Requests)}.[{i}].{nameof(Order.OrderType)}", "Invalid Upload Type");
                    }
                }
                else if (order.Requests[i].OrderType == 2)//For Outbound
                {
                    if (string.IsNullOrEmpty(order.Requests[i].ShipmentSAPNo))
                        ModelState.AddModelError($"{nameof(order)}.{nameof(order.Requests)}.[{i}].{nameof(Order.ShipmentSAPNo)}", "Invalid Shipment SAP Number");
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
                        ErrorMessage = ModelState.Keys.ToList<string>()[i].Replace("order.Requests[", "Row Number[") + " : " + ModelState.Values.ToList<ModelState>()[i].Errors[0].ErrorMessage + " : " + ModelState.Values.ToList<ModelState>()[i].Errors[0].Exception.Message
                    };

                    errorResponse.Data.Add(errorData);
                }
                return Ok(errorResponse);
            }

            IOrderTask orderTask = Helper.Model.DependencyResolver.DependencyResolver.GetImplementationOf<ITaskGateway>().OrderTask;
            OrderResponse orderData = orderTask.CreateUpdateOrder(order);

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
    }
}
