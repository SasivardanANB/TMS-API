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
    public class ShipmentListDetails
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public int NumberOfBoxes { get; set; }
        public string Note { get; set; }
        public string PackingSheetNumber { get; set; }
        [ForeignKey("TripDetail")]
        public int StopPointId { get; set; }

        public TripDetail TripDetail { get; set; }
        public string CreatedBy
        {
            get;
            set;
        }
        public DateTime? CreatedTime
        {
            get;
            set;
        }
        public string LastModifiedBy { get; set; }
        public DateTime? LastModifiedTime { get; set; }
    }
}
