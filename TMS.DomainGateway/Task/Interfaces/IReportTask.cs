using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMS.DomainObjects.Request;
using TMS.DomainObjects.Response;

namespace TMS.DomainGateway.Task.Interfaces
{
    public interface IReportTask
    {
        OrderReportResponse OrdersDayWiseReport(OrderReportRequest orderReportRequest);
        OrderReportResponse OrdersProgress(OrderReportRequest orderReportRequest);
        OrderReportResponse FinishedOrderReports(OrderReportRequest orderReportRequest);
        OrderReportResponse OrdersLoadAndUnloadAvgDayWiseReport(OrderReportRequest orderReportRequest);
        GoodsReceiveOrIssueResponse GoodsReceiveOrGoodsIssueReport(GoodsReceiveOrIssueRequest goodsReceiveOrIssueRequest);
        AdminBoardReportResponse BoardAdminReport(int orderTypeId);
    }
}
