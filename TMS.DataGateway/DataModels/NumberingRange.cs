using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMS.DataGateway.DataModels
{
    [Table("NumberingRange", Schema = "TMS")]
    public class NumberingRange
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [Required(ErrorMessage = "")]
        [MaxLength(10)]
        [Index("NumberingRange_CompanyCodeCode", IsUnique = true)]
        public string CompanyCodeCode { get; set; }
        [MaxLength(10)]
        [Index("NumberingRange_BusinessAreaCode", IsUnique = true)]
        public string BusinessAreaCode { get; set; }
        [MaxLength(10)]
        [Index("NumberingRange_TransactionTypeCode", IsUnique = true)]
        public string TransactionTypeCode { get; set; }
        public string Prefix { get; set; }
        public int StartNumber { get; set; }
        public int EndNumber { get; set; }
        public int LastNumber { get; set; }
    }
}
