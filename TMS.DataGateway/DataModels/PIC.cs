using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMS.DataGateway.DataModels
{
    [Table("PIC", Schema = "TMS")]
    public class PIC
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [MaxLength(50)]
        public string PICName { get; set; }
        
        [MaxLength(15)]
        public string PICPhone { get; set; }
        [MaxLength(50)]
        public string PICEmail { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        [MaxLength(50)]
        public string PICPassword { get; set; }
        [ForeignKey("ImageGuid")]
        public int PhotoId { get; set; }
        public virtual ImageGuid ImageGuid { get; set; }
    }
}
