using NLog;
using System;
using System.Net;
using TMS.DataGateway.Repositories.Interfaces;
using TMS.DomainObjects.Request;
using TMS.DomainObjects.Response;

namespace TMS.DataGateway.Repositories
{
    public class Invoice : IInvoice
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public InvoiceResponse GenerateInvoice(InvoiceRequest invoiceRequest)
        {
            InvoiceResponse invoiceResponse  = new InvoiceResponse();
            try
            {
                throw new NotImplementedException();
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, ex);
                invoiceResponse.Status = DomainObjects.Resource.ResourceData.Failure;
                invoiceResponse.StatusCode = (int)HttpStatusCode.ExpectationFailed;
                invoiceResponse.StatusMessage = ex.Message;
            }
            return invoiceResponse;
        }
    }
}
