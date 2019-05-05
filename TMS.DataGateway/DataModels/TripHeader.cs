using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMS.DataGateway.DataModels
{
    [Table("TripHeader", Schema = "TMS")]
    public class TripHeader
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public int BusinessAreaID { get; set; }
        [MaxLength(10)]
        [Index("TripHeader_TripNo", IsUnique = true)]
        public string TripNo { get; set; }
        public int TripStatusID { get; set; }
        public int TripTypeID { get; set; }
        public DateTime TripDate { get; set; }
        public string Notes { get; set; }
        public DateTime EstimatedTripTime { get; set; }
        public DateTime ActualTripTime { get; set; }
        public decimal TotalWeight { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal TotalKM { get; set; }
        public int ReferenceTripNo { get; set; }
        public int IsParent { get; set; }
    }
}
