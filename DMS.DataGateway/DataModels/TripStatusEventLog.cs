using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DMS.DataGateway.DataModels
{
    [Table("TripEventLog", Schema = "DMS")]
    public class TripStatusEventLog
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [ForeignKey("StopPoints")]
        public int StopPointId { get; set; }
        public virtual StopPoints StopPoints { get; set; }
        public DateTime StatusDate { get; set; }
        public string Remarks { get; set; }
        [ForeignKey("TripStatus")]
        public int TripStatusId { get; set; }
        public virtual TripStatus TripStatus { get; set; }
    }
}
