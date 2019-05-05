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
        [ForeignKey("StopPoints")]
        public int StopPointId { get; set; }

        public StopPoints StopPoints { get; set; }
    }
}
