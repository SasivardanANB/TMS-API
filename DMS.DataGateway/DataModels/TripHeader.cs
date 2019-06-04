using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DMS.DataGateway.DataModels
{
    [Table("TripHeader", Schema = "DMS")]
    public class TripHeader
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [MaxLength(50)]
        [Index("TripNumber", IsUnique = true)]
        public string TripNumber { get; set; }
        [MaxLength(50)]
        public string OrderNumber { get; set; }
        [MaxLength(50)]
        public string TransporterName { get; set; }
        public string TransporterCode { get; set; }
        [ForeignKey("Driver")]
        public int DriverId { get; set; }
        public Driver Driver { get; set; }
        public string VehicleType { get; set; }
        public string VehicleNumber { get; set; }
        public string TripType { get; set; }
        public decimal Weight { get; set; }
        public string PoliceNumber { get; set; }
        [ForeignKey("TripStatus")]
        public int? CurrentTripStatusId { get; set; }
        public virtual TripStatus TripStatus { get; set; }
        public int OrderType { get; set; }
        public DateTime TripDate { get; set; }
        [ForeignKey("BusinessArea")]
        public int BusinessAreaId { get; set; }
        public BusinessArea BusinessArea { get; set; }
        public string CreatedBy
        {
            get;
            set;
        }
        public DateTime? CreatedTime
        {
            get;
            set;
        }
        public string LastModifiedBy { get; set; }
        public DateTime? LastModifiedTime { get; set; }
    }
}
