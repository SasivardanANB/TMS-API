using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMS.DomainObjects.Objects
{
    public class GoodsReceiveOrIssueReport
    {
        public int OrderTypeId { get; set; }
        public int PartnerId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<GoodsReceiveOrIssue> GoodsReceiveOrIssues { get; set; }
    }
    public class GoodsReceiveOrIssue
    {
        public DateTime Date { get; set; }
        public string GRQty { get; set; }
        public string GIQty { get; set; }
        public string OrderQty { get; set; }
        public string Percentage { get; set; }
    }
}
