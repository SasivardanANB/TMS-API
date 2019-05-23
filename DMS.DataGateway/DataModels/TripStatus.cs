using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DMS.DataGateway.DataModels
{
    [Table("TripStatus", Schema = "DMS")]
    public class TripStatus
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public string StatusCode { get; set; }
        public string StatusName { get; set; }
    }
}
