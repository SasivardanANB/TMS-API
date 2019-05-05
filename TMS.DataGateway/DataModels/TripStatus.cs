using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMS.DataGateway.DataModels
{
    [Table("TripStatus", Schema = "TMS")]
    public class TripStatus
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [MaxLength(10)]
        [Index("TripStatus_TripStatusCode", IsUnique = true)]
        public string TripStatusCode { get; set; }
        public string TripStatusDescription { get; set; }
    }
}
