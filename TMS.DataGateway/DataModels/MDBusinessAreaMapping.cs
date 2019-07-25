using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TMS.DataGateway.DataModels
{
    [Table("MDBusinessAreaMapping", Schema = "TMS")]
    public class MDBusinessAreaMapping : ModelBase
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public string MainDealerCode { get; set; }
        public string BusinessAreaCode { get; set; }
    }
}
