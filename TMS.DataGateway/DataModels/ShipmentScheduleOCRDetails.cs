using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMS.DataGateway.DataModels
{
    [Table("ShipmentScheduleOCRDetails", Schema = "TMS")]
    public class ShipmentScheduleOCRDetails 
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [MaxLength(250)]
        public string EmailFrom { get; set; }
        public DateTime EmailDateTime { get; set; }

        public string ShipmentScheduleNo { get; set; }
        public string DayShipment { get; set; }
        public string ShipmentTime { get; set; }
        public string VehicleType { get; set; }
        public string MainDealerCode { get; set; }
        public string MainDealerName { get; set; }
        public string ShipToParty { get; set; }
        public string MultiDropShipment { get; set; }
        public string EstimatedTotalPallet { get; set; }
        public string Weight { get; set; }

        public bool IsProcessed { get; set; }
        public string ProcessMessage { get; set; }
        public bool IsOrderCreated { get; set; }
        public string ImageGuid { get; set; }
        public DateTime ProcessedDateTime { get; set; }
        public string ProcessedBy { get; set; }
    }
}
