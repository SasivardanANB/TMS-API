using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMS.DataGateway.DataModels
{
    [Table("TermsOfPayment", Schema = "TMS")]
    public class TermsOfPayment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [MaxLength(10)]
        [Index("TermsOfPayment_TermsOfPaymentCode", IsUnique = true)]
        public string TermsOfPaymentCode { get; set; }
        public string TermsOfPaymentDescription { get; set; }
    }
}
