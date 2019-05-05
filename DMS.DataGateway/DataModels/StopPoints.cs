using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DMS.DataGateway.DataModels
{
    [Table("StopPoints", Schema = "DMS")]
    public class StopPoints
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [ForeignKey("TripDetails")]
        public int TripID { get; set; }
        [ForeignKey("Location")]
        public int LocationID { get; set; }
        public int SequenceNumber { get; set; }
        public DateTime ActualDeliveryDate { get; set; }    
        public DateTime EstimatedDeliveryDate { get; set; }

        public Location Location { get; set; }
        public TripDetails TripDetails { get; set; }

    }
}
