using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMS.DataGateway.DataModels
{
    [Table("ImageType", Schema = "DMS")]
    public class ImageType : ModelBase
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public int ImageTypeCode { get; set; }
        public string ImageTypeDescription { get; set; }
        
    }
}
