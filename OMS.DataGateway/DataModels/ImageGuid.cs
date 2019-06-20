using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OMS.DataGateway.DataModels
{
    [Table("ImageGuid", Schema = "OMS")]
    public class ImageGuid
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [MaxLength(1000)]
        public string ImageGuIdValue { get; set; }
        public bool IsActive { get; set; }
    }
}
