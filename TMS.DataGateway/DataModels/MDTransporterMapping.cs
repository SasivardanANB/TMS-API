using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TMS.DataGateway.DataModels
{
    [Table("MDTransporterMapping", Schema = "TMS")]
    public class MDTransporterMapping : ModelBase
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public int DestinationPartnerID { get; set; }
        public int TransporterID { get; set; }
        public int Priority { get; set; }
    }
}
