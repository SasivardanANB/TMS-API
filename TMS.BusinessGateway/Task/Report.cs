using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMS.DataGateway.Repositories.Interfaces;
using TMS.DomainGateway.Task;
using TMS.DomainObjects.Request;
using TMS.DomainObjects.Response;

namespace TMS.BusinessGateway.Task
{
    public partial class BusinessReportTask : ReportTask
    {
        private readonly IReport _reportRepository;

        public BusinessReportTask(IReport reportRepository)
        {
            _reportRepository = reportRepository;
        }

        public override OrderReportResponse OrdersDayWiseReport(OrderReportRequest orderReportRequest)
        {
            OrderReportResponse orderReportResponse = _reportRepository.OrdersDayWiseReport(orderReportRequest);
            return orderReportResponse;
        }

        public override OrderReportResponse OrdersProgress(OrderReportRequest orderReportRequest)
        {
            OrderReportResponse orderReportResponse = _reportRepository.OrdersProgress(orderReportRequest);
            return orderReportResponse;
        }

        public override OrderReportResponse FinishedOrderReports(OrderReportRequest orderReportRequest)
        {
            OrderReportResponse orderReportResponse = _reportRepository.FinishedOrderReports(orderReportRequest);
            return orderReportResponse;
        }

        public override OrderReportResponse OrdersLoadAndUnloadAvgDayWiseReport(OrderReportRequest orderReportRequest)
        {
            OrderReportResponse orderReportResponse = _reportRepository.OrdersLoadAndUnloadAvgDayWiseReport(orderReportRequest);
            return orderReportResponse;
        }

        public override GoodsReceiveOrIssueResponse GoodsReceiveOrGoodsIssueReport(GoodsReceiveOrIssueRequest goodsReceiveOrIssueRequest)
        {
            GoodsReceiveOrIssueResponse goodsReceiveOrIssueResponse = _reportRepository.GoodsReceiveOrGoodsIssueReport(goodsReceiveOrIssueRequest);
            return goodsReceiveOrIssueResponse;
        }

        public override AdminBoardReportResponse BoardAdminReprt(int orderTypeId)
        {
            AdminBoardReportResponse adminBoardReportResponse = _reportRepository.BoardAdminReprt(orderTypeId);
            return adminBoardReportResponse;
        }
    }
}
