using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMS.DataGateway.DataModels
{
    [Table("TripGuid", Schema = "DMS")]
    public class TripGuid
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [ForeignKey("TripStatusEventLog")]
        public int TripEventLogID { get; set; }
        public virtual TripStatusEventLog TripStatusEventLog { get; set; }
        [ForeignKey("ImageGuid")]
        public int ImageID { get; set; }
        public virtual ImageGuid ImageGuid { get; set; }
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
