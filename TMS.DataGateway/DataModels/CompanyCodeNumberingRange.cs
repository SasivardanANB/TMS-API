using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMS.DataGateway.DataModels
{
    [Table("CompanyCodeNumberingRange", Schema = "TMS")]
    public class CompanyCodeNumberingRange
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [Required(ErrorMessage = "")]
        [MaxLength(4)]
        [Index("CompanyCodeNumberingRange_CompanyCodeCode", IsUnique = true)]
        public string CompanyCodeCode { get; set; }
        [MaxLength(10)]
        [Index("CompanyCodeNumberingRange_BusinessAreaCode", IsUnique = true)]
        public string BusinessAreaCode { get; set; }
        public int IsNumberRange { get; set; }
        [MaxLength(10)]
        [Index("CompanyCodeNumberingRange_TransactionTypeCode", IsUnique = true)]
        public string TransactionTypeCode { get; set; }
        public int IsDisplayOnly { get; set; }
    }
}
