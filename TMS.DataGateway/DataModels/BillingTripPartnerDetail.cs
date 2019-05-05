using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMS.DataGateway.DataModels
{
    [Table("BillingTripPartnerDetail", Schema = "TMS")]
    public class BillingTripPartnerDetail
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [Index("BillingTripPartnerDetail_BillingDetailID", IsUnique = true)]
        public int BillingDetailID { get; set; }
        [Index("BillingTripPartnerDetail_PartnerTypeID", IsUnique = true)]
        public int PartnerTypeID { get; set; }
        [Index("BillingTripPartnerDetail_PartnerID", IsUnique = true)]
        public int PartnerID { get; set; }
        public int ParentID { get; set; }
        public int IsParent { get; set; }
        public int IsOriginal { get; set; }
    }
}
