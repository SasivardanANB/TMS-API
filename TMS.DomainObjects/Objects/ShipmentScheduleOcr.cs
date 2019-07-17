using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMS.DomainObjects.Objects
{
    public class ShipmentScheduleOcr
    {
        public bool Success { get; set; }
        public string DocumentType { get; set; }
        public DetailsOcr Data { get; set; }
        public string Image { get; set; }
    }

    public class DetailsOcr
    {
        public string ShipmentScheduleNo {get;set;} 
        public string DayShipment {get;set;}
        public string ShipmentTime {get;set;}
        public string VehicleType {get;set;}
        public string MainDealerCode {get;set;}
        public string MainDealerName {get;set;}
        public string ShipToParty  {get;set;}
        public string MultiDropShipment  {get;set;}
        public string EstimatedTotalPall {get;set;} 
        public string Weight {get;set;}
    }
}
