using NLog;
using OMS.API.Classes;
using OMS.DomainGateway.Gateway;
using OMS.DomainGateway.Gateway.Interfaces;
using OMS.DomainGateway.Task;
using OMS.DomainGateway.Task.Interfaces;
using OMS.DomainObjects.Objects;
using OMS.DomainObjects.Request;
using OMS.DomainObjects.Response;
using System;
using System.Collections.Generic;
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

            return Ok(orderData);
        }
    }
}
