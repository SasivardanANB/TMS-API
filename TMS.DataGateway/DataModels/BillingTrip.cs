using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMS.DataGateway.DataModels
{
    [Table("BillingTrip", Schema = "TMS")]
    public class BillingTrip
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [Index("BillingTrip_CompanyCodeID", IsUnique = true)]
        public int CompanyCodeID { get; set; }
        [Index("BillingTrip_BusinessAreaID", IsUnique = true)]
        public int BusinessAreaID { get; set; }
        [MaxLength(10)]
        [Index("BillingTrip_BillingNo", IsUnique = true)]
        public string BillingNo { get; set; }
        public DateTime BillingDate { get; set; }
        public int BillingTypeID { get; set; }
        public int BillingStatusID { get; set; }
        public int TermsOfPaymentID { get; set; }
        public int CancellationReasonID { get; set; }
        public string CancellationReasonDescription { get; set; }
        public decimal TotalNetValue { get; set; }
        public decimal TotalVATAmount { get; set; }
        public decimal TotalDiscount { get; set; }
        public decimal TotalIncVAT { get; set; }
    }
}
