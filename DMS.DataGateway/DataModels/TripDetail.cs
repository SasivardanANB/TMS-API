using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DMS.DataGateway.DataModels
{
    [Table("TripDetail", Schema = "DMS")]
    public class TripDetail
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [ForeignKey("TripHeader")]
        public int TripID { get; set; }
        public TripHeader TripHeader { get; set; }
        [ForeignKey("Partner")]
        public int PartnerId { get; set; }
        public Partner Partner { get; set; }
        public int SequenceNumber { get; set; }
        public DateTime ActualDeliveryDate { get; set; }    
        public DateTime EstimatedDeliveryDate { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public string CreatedBy
        {
            get { return "SYSTEM"; }
            set { }
        }
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
