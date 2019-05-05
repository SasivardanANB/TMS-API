using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMS.DataGateway.DataModels
{
    [Table("BillingTripDetail", Schema = "TMS")]
    public class BillingTripDetail
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [Index("BillingTripDetail_BillingHeaderID", IsUnique = true)]
        public int BillingHeaderID { get; set; }
        [Index("BillingTripDetail_ItemNo", IsUnique = true)]
        public int ItemNo { get; set; }
        [Index("BillingTripDetail_TripDetailID", IsUnique = true)]
        public int TripDetailID { get; set; }
        public int ItemStatus { get; set; }
        public decimal Berat { get; set; }
        public decimal KM { get; set; }
        public decimal Price { get; set; }
        public decimal Discount { get; set; }
        public decimal VATAmount { get; set; }
    }
}
