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
    public class StopPointImages : ModelBase
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [ForeignKey("TripDetail")]
        public int StopPointId { get; set; }
        public TripDetail TripDetail { get; set; }
        [ForeignKey("ImageGuId")]
        public int ImageId { get; set; }
        public ImageGuId ImageGuId { get; set; }
        [ForeignKey("ImageType")]
        public int ImageTypeId { get; set; }
        public ImageType ImageType { get; set; }
       
    }
}
