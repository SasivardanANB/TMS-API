using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMS.DataGateway.DataModels
{
    [Table("G2G", Schema = "TMS")]
    public class G2G
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [ForeignKey("BusinessArea")]
        public int BusinessAreaId { get; set; }
        public virtual BusinessArea BusinessArea { get; set; }
        [MaxLength(50)]
        public string G2GName { get; set; }
        [ForeignKey("GateType")]
        public int GateTypeId { get; set; }
        public virtual GateType GateType { get; set; }
    }
}
