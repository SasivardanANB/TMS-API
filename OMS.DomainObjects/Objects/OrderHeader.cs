using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMS.DomainObjects.Objects
{
    public class OrderHeader
    {
        public int ID { get; set; }
        public string CompanyCodeID { get; set; }
        public string BusinessAreaID { get; set; }
        public string TipeOrder { get; set; }
        public string OrderNo { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime EstimatedPickupTime { get; set; }
        public DateTime ActualPickupTime { get; set; }
        public DateTime EstimatedArrivalTime { get; set; }
        public DateTime ActualArrivalTime { get; set; }
        public decimal TotalPrice { get; set; }
        public int OrderStatusID { get; set; }
        public string Sender { get; set; }
        public string Receiver { get; set; }
        public string Instruction { get; set; }
        public string VehicleType { get; set; }
        public string PoliceNo { get; set; }
        public int TotalOfWeight { get; set; }
    }
}
