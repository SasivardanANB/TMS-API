using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMS.DataGateway.DataModels
{
    [Table("CancellationReason", Schema = "TMS")]
    public class CancellationReason
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [MaxLength(10)]
        [Index("CancellationReason_CancellationReasonCode", IsUnique = true)]
        public string CancellationReasonCode { get; set; }
        public string CancellationReasonDescription { get; set; }
    }
}
