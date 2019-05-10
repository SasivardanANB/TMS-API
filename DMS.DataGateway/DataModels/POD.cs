using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DMS.DataGateway.DataModels
{
    [Table("POD", Schema = "DMS")]
    public class Pod
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [MaxLength(25), Required]
        public string TripNo { get; set; }
        public int SourcePointId { get; set; }
        public int DestinationPointId { get; set; }
        public int OrderTypeId { get; set; }
        [MaxLength(25), Required]
        public string ShipmentNo { get; set; }
        [MaxLength(20), Required]
        public string DoosNo { get; set; }
        [MaxLength(20), Required]
        public string MaterialNo { get; set; }
        [MaxLength(100), Required]
        public string MaterialDescription { get; set; }
        [MaxLength(7), Required]
        public string Weight { get; set; }
        [MaxLength(12), Required]
        public string PoliceNo { get; set; }
        public int TripStatusId { get; set; }
        public int PODImageId { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime CreatedTime
        {
            get { return DateTime.Now; }
            set { }
        }
        public string LastModifiedBy { get; set; }
        public DateTime? LastModifiedTime { get; set; }
    }
}
