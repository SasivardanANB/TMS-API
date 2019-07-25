using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TMS.DataGateway.DataModels
{
    [Table("Harga", Schema = "TMS")]
    public class Harga : ModelBase
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public int TransporterID { get; set; }
        public int VechicleTypeID { get; set; }
        public decimal Price { get; set; }
    }
}
