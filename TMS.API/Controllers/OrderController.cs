﻿using NLog;
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
        public IHttpActionResult CreateUpdateOrder(OrderRequest request)
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
            OrderResponse orderData = orderTask.CreateUpdateOrder(request);

            return Ok(orderData);
        }

        [Route("getorders")]
        [HttpPost]
        public IHttpActionResult GetOrders(OrderRequest order)
        {
            throw new NotImplementedException();
        }
    }
}