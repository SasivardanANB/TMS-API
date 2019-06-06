using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMS.DomainObjects.Objects
{
    public class StopPoints
    {
        public int ID { get; set; }
        public int TripId { get; set; }
        public int LocationId  { get; set; }
        public string LocationName { get; set; }
        public int SequenceNumber  { get; set; }
        public DateTime ActualDeliveryDate { get; set; }
        public DateTime EstimatedDeliveryDate { get; set; }
        public string TripStatusCode { get; set; }
    }
}
