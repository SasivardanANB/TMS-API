using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMS.DomainGateway.Task.Interfaces;
using TMS.DomainObjects.Request;
using TMS.DomainObjects.Response;

namespace TMS.DomainGateway.Task
{
    public abstract class ReportTask : IReportTask
    {
        public abstract OrderReportResponse OrdersDayWiseReport(OrderReportRequest orderReportRequest);
        public abstract OrderReportResponse OrdersProgress(OrderReportRequest orderReportRequest);
        public abstract OrderReportResponse FinishedOrderReports(OrderReportRequest orderReportRequest);
        public abstract OrderReportResponse OrdersLoadAndUnloadAvgDayWiseReport(OrderReportRequest orderReportRequest);
        public abstract GoodsReceiveOrIssueResponse GoodsReceiveOrGoodsIssueReport(GoodsReceiveOrIssueRequest goodsReceiveOrIssueRequest);
        public abstract AdminBoardReportResponse BoardAdminReprt(int orderTypeId);
    }
}
