using AutoMapper;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using TMS.DataGateway.DataModels;
using TMS.DataGateway.Repositories.Interfaces;
using TMS.DomainObjects.Request;
using TMS.DomainObjects.Response;
using Domain = TMS.DomainObjects.Objects;
using DataModel = TMS.DataGateway.DataModels;


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
                //using (var tMSDBContext = new TMSDBContext())
                //{
                //    invoiceResponse.Data = invoiceRequest.Requests;
                //    invoiceResponse.Status = DomainObjects.Resource.ResourceData.Success;
                //    invoiceResponse.StatusCode = (int)HttpStatusCode.OK;
                //}
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
