using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMS.DataGateway.DataModels
{
    [Table("GateInGateOut", Schema = "TMS")]
    public class GateInGateOut
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public int OrderId { get; set; }
        [ForeignKey("G2G")]
        public int G2GId { get; set; }
        public virtual G2G G2G { get; set; }
        [ForeignKey("GateType")]
        public int GateTypeId { get; set; }
        public virtual GateType GateType { get; set; }
        [MaxLength(120)]
        public string Info { get; set; }
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
