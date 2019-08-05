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

        [Route("getgoodsreceivereport")]
        [HttpPost]
        public IHttpActionResult GetGoodsReceiveReport(GoodsReceiveOrIssueRequest goodsReceiveOrIssueRequest)
        {
            goodsReceiveOrIssueRequest.Request.OrderTypeId = 1;
            IReportTask reportTask = DependencyResolver.GetImplementationOf<ITaskGateway>().ReportTask;
            GoodsReceiveOrIssueResponse orderReportResponse = reportTask.GoodsReceiveOrGoodsIssueReport(goodsReceiveOrIssueRequest);
            return Ok(orderReportResponse);
        }

        [Route("getgoodsissuereport")]
        [HttpPost]
        public IHttpActionResult GetGoodsIssueReport(GoodsReceiveOrIssueRequest goodsReceiveOrIssueRequest)
        {
            goodsReceiveOrIssueRequest.Request.OrderTypeId = 2;
            IReportTask reportTask = DependencyResolver.GetImplementationOf<ITaskGateway>().ReportTask;
            GoodsReceiveOrIssueResponse orderReportResponse = reportTask.GoodsReceiveOrGoodsIssueReport(goodsReceiveOrIssueRequest);
            return Ok(orderReportResponse);
        }

        [Route("inboundboardadminreport")]
        [HttpGet]
        public IHttpActionResult InBoundBoardAdminReprt()
        {
            int orderTypeId = 1;
            IReportTask reportTask = DependencyResolver.GetImplementationOf<ITaskGateway>().ReportTask;
            AdminBoardReportResponse orderReportResponse = reportTask.BoardAdminReport(orderTypeId);
            return Ok(orderReportResponse);
        }

        [Route("outboundboardadminreport")]
        [HttpGet]
        public IHttpActionResult OutBoundBoardAdminReprt()
        {
            int orderTypeId = 2;
            IReportTask reportTask = DependencyResolver.GetImplementationOf<ITaskGateway>().ReportTask;
            AdminBoardReportResponse orderReportResponse = reportTask.BoardAdminReport(orderTypeId);
            return Ok(orderReportResponse);
        }
    }
}
