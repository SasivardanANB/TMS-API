using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMS.DataGateway.DataModels
{
    [Table("OrderPartnerDetail", Schema = "TMS")]
    public class OrderPartnerDetail
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [Index("OrderPartnerDetail_OrderDetailID", IsUnique = true)]
        public int OrderDetailID { get; set; }
        [Index("OrderPartnerDetail_PartnerTypeID", IsUnique = true)]
        public int PartnerTypeID { get; set; }
        [Index("OrderPartnerDetail_PartnerID", IsUnique = true)]
        public int PartnerID { get; set; }
        public int ParentID { get; set; }
        public int IsParent { get; set; }
        public int IsOriginal { get; set; }
        public string CustomerName { get; set; }
        public string CustomerPhone { get; set; }
        [MaxLength(200)]
        public string CustomerAddress { get; set; }
        public string Longitude { get; set; }
        public string Lattitude { get; set; }
    }
}
