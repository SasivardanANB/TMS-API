using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMS.DataGateway.DataModels
{
    [Table("TripType", Schema = "TMS")]
    public class TripType
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [MaxLength(10)]
        [Index("TripType_TripTypeCode", IsUnique = true)]
        public string TripTypeCode { get; set; }
        public string TripTypeDescription { get; set; }
    }
}
