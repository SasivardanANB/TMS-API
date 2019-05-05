using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMS.DataGateway.DataModels
{
    [Table("Expeditor", Schema = "TMS")]
    public class Expeditor
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [MaxLength(20)]
        public string Initial { get; set; }
        [MaxLength(50)]
        public string ExpeditorName { get; set; }
        [MaxLength(50)]
        public string ExpeditorEmail { get; set; }
        [MaxLength(255)]
        public string Address { get; set; }
        public int PostalCodeID { get; set; }
        [ForeignKey("PostalCodeID")]
        public virtual PostalCode PostalCode { get; set; }
        //[ForeignKey("PIC")]
        public int PICID { get; set; }
        public bool TypeCode { get; set; }
        public int ExpeditorTypeID { get; set; }
        [ForeignKey("ExpeditorTypeID")]
        public virtual ExpeditorType ExpeditorType { get; set; }
    }
}
