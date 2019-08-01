using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMS.DomainObjects.Objects;

namespace TMS.DomainObjects.Response
{
    public class OrderDetailsResponse : Message
    {
        public string SOPONumber { get; set; }
        public string LegecyOrderNo { get; set; }
        public int ID { get; set; }
        public string BusinessArea { get; set; }
        public int BusinessAreaId { get; set; }
        public string OrderNo { get; set; }
        public int SequenceNo { get; set; }
        public int FleetType { get; set; }
        public int OrderType { get; set; }
        public string VehicleShipmentType { get; set; }
        public string DriverNo { get; set; }
        public string DriverName { get; set; }
        public string VehicleNo { get; set; }
        public decimal OrderWeight { get; set; }
        public string OrderWeightUM { get; set; }
        public string EstimationShipmentDate { get; set; }
        public string EstimationShipmentTime { get; set; }
        public string ActualShipmentDate { get; set; }
        public string ActualShipmentTime { get; set; }
        public string Sender { get; set; }
        public string Receiver { get; set; }
        public int OrderShipmentStatus { get; set; }
        public string Dimension { get; set; }
        public int TotalPallet { get; set; }
        public string Instructions { get; set; }
        public string ShippingListNo { get; set; }
        public string PackingSheetNo { get; set; }
        public int TotalCollie { get; set; }
        public string ShipmentSAPNo { get; set; }
        public bool IsActive { get; set; }
        public int OrderDetailID { get; set; }
        public decimal Harga { get; set; }
        public int? ShipmentScheduleImageID { get; set; }
        public string ShipmentScheduleImageGUID { get; set; }
        public StopPoints Transporter { get; set; }
        public List<StopPoints> SourceOrDestinations { get; set; }
        public bool IsPackinsSheetAvailable { get; set; }
    }
}
