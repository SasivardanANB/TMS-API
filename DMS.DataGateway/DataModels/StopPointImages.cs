using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DMS.DataGateway.DataModels
{
    [Table("StopPointImages", Schema = "DMS")]
    public class StopPointImages
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [ForeignKey("StopPoints")]
        public int StopPointId { get; set; }
        public string ImageGUId { get; set; }
        public string ImageType { get; set; }

        public StopPoints StopPoints { get; set; }
    }
}
