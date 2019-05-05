using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMS.DataGateway.DataModels
{
    [Table("TripStatusHistory", Schema = "TMS")]
    public class TripStatusHistory
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [Index("TripStatusHistory_TripHeaderID", IsUnique = true)]
        public int TripHeaderID { get; set; }
        [Index("TripStatusHistory_TripStatusID", IsUnique = true)]
        public int TripStatusID { get; set; }
        [Index("TripStatusHistory_StepNo", IsUnique = true)]
        public int StepNo { get; set; }
        public int IsOptional { get; set; }
        public DateTime CreatedTime { get; set; }
        public DateTime CreatedBy { get; set; }
        public DateTime LastModififiedTime { get; set; }
        public DateTime LastModifiedBy { get; set; }
    }
}
