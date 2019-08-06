using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using Helper.Model.DependencyResolver;
using System.Web.Http;
using TMS.DomainGateway.Task.Interfaces;
using TMS.DomainObjects.Request;
using TMS.DomainObjects.Response;
using TMS.DomainGateway.Gateway.Interfaces;
using TMS.API.Classes;

namespace TMS.API.Controllers
{
    [CustomAuthorize]
    [RoutePrefix("api/v1/invoice")]
    public class InvoiceController : ApiController
    {
        [Route("generateinvoice"),AllowAnonymous]
        [HttpPost]
        public IHttpActionResult GenerateInvoice(InvoiceRequest invoiceRequest)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            IInvoiceTask invoiceTask = DependencyResolver.GetImplementationOf<ITaskGateway>().InvoiceTask;
            InvoiceResponse invoiceResponse = invoiceTask.GenerateInvoice(invoiceRequest);
            return Ok(invoiceResponse);
        }
    }
}
