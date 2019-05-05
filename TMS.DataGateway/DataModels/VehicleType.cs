using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMS.DataGateway.DataModels
{
    [Table("VehicleType", Schema = "TMS")]
    public class VehicleType
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [MaxLength(10)]
        [Index("VehicleType_VehicleTypeCode", IsUnique = true)]
        public string VehicleTypeCode { get; set; }
        public string VehicleTypeDescription { get; set; }
    }
}
