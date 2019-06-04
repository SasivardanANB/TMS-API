using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMS.DataGateway.DataModels
{
    [Table("ImageGuid", Schema = "DMS")]
    public class ImageGuId
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [MaxLength(1000)]
        public string ImageGuIdValue { get; set; }
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
        public bool IsActive { get; set; }
    }
}
