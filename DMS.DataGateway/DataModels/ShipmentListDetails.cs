using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DMS.DataGateway.DataModels
{
    [Table("ShipmentList", Schema = "DMS")]
    public class ShipmentListDetails : ModelBase
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [MaxLength(20)]
        public string ShippingListNo { get; set; }
        public int NumberOfBoxes { get; set; }
        [MaxLength(200)]
        public string Note { get; set; }
        [MaxLength(20)]
        public string PackingSheetNumber { get; set; }
        [ForeignKey("TripDetail")]
        public int StopPointId { get; set; }
        public TripDetail TripDetail { get; set; }
       
    }
}
