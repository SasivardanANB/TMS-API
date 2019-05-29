using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMS.DomainObjects.Objects
{
    public class OrderReport
    {
        public int OrderTypeId { get; set; }
        public int MainDealerId { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public List<OrdersByDate> OrdersByDates { get; set; }
        public List<OrderProgress> orderProgresses { get; set; }

    }
    public class OrdersByDate
    {
        public int Day { get; set; }
        public int OrderCount { get; set; }
    }
    public class OrderProgress
    {
        public int OrderId { get; set; }
        public string OrderNo { get; set; }
        public string Transporter { get; set; }
        public string Source { get; set; }
        public string Destination { get; set; }
        public string Drivername { get; set; }
        public string Vehicle { get; set; }
        public string OrderCreatedDate { get; set; }
        public int OrderStatusId { get; set; }
        public string OrderStatus { get; set; }
    }
}
