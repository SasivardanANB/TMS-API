using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DMS.DomainObjects.Objects;

namespace DMS.DataGateway.DataModels
{
    [Table("DeviceToken", Schema = "DMS")]
    public class DeviceToken : ModelBase
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [ForeignKey("Driver")]
        public int DriverId { get; set; }
        public Driver Driver { get; set; }
        [MaxLength(250)]
        public string DeviceKey { get; set; }
    }
}
