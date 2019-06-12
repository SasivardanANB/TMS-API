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
    [RoutePrefix("api/v1/report")]
    public class ReportController : ApiController
    {
        [Route("getordersdatewise")]
        [HttpPost]
        public IHttpActionResult OrdersDayWiseReport(OrderReportRequest orderReportRequest)
        {
            IReportTask reportTask = DependencyResolver.GetImplementationOf<ITaskGateway>().ReportTask;
            OrderReportResponse orderReportResponse = reportTask.OrdersDayWiseReport(orderReportRequest);
            return Ok(orderReportResponse);
        }

        [Route("getordersprogress")]
        [HttpPost]
        public IHttpActionResult OrdersProgress(OrderReportRequest orderReportRequest)
        {
            IReportTask reportTask = DependencyResolver.GetImplementationOf<ITaskGateway>().ReportTask;
            OrderReportResponse orderReportResponse = reportTask.OrdersProgress(orderReportRequest);
            return Ok(orderReportResponse);
        }

        [Route("finishedorderreports")]
        [HttpPost]
        public IHttpActionResult FinishedOrderReports(OrderReportRequest orderReportRequest)
        {
            IReportTask reportTask = DependencyResolver.GetImplementationOf<ITaskGateway>().ReportTask;
            OrderReportResponse orderReportResponse = reportTask.FinishedOrderReports(orderReportRequest);
            return Ok(orderReportResponse);
        }

        [Route("avgloadingperdayreport")]
        [HttpPost]
        public IHttpActionResult AvgLoadingPerDayReport(OrderReportRequest orderReportRequest)
        {
            orderReportRequest.Request.OrderAvgLoadingTypeId = 1;

            IReportTask reportTask = DependencyResolver.GetImplementationOf<ITaskGateway>().ReportTask;
            OrderReportResponse orderReportResponse = reportTask.OrdersLoadAndUnloadAvgDayWiseReport(orderReportRequest);
            return Ok(orderReportResponse);
        }

        [Route("avgunloadingperdayreport")]
        [HttpPost]
        public IHttpActionResult AvgUnloadingPerDayReport(OrderReportRequest orderReportRequest)
        {
            orderReportRequest.Request.OrderAvgLoadingTypeId = 2;
            IReportTask reportTask = DependencyResolver.GetImplementationOf<ITaskGateway>().ReportTask;
            OrderReportResponse orderReportResponse = reportTask.OrdersLoadAndUnloadAvgDayWiseReport(orderReportRequest);
            return Ok(orderReportResponse);
        }
    }
}
