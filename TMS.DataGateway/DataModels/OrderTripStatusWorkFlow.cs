using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMS.DataGateway.DataModels
{
    [Table("OrderTripStatusWorkFlow", Schema = "TMS")]
    public class OrderTripStatusWorkFlow
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [Index("OrderTripStatusWorkFlow_TripTypeID", IsUnique = true)]
        public int TripTypeID { get; set; }
        [Index("OrderTripStatusWorkFlow_TripStatusID", IsUnique = true)]
        public int TripStatusID { get; set; }
        [Index("OrderTripStatusWorkFlow_StepNo", IsUnique = true)]
        public int StepNo { get; set; }
        public int IsOptional { get; set; }
        public int IsTrackable { get; set; }

    }
}
