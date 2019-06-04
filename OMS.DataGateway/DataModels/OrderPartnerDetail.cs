using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMS.DataGateway.DataModels
{
    [Table("OrderPartnerDetail", Schema = "OMS")]
    public class OrderPartnerDetail : ModelBase
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [ForeignKey("OrderDetail")]
        public int OrderDetailID { get; set; }
        public OrderDetail OrderDetail { get; set; }
        [ForeignKey("Partner")]
        public int PartnerID { get; set; }
        public Partner Partner { get; set; }
        public bool IsParent { get; set; }
        public bool IsOriginal { get; set; }
        [ForeignKey("PartnerType")]
        public int PartnerTypeId { get; set; }
        public PartnerType PartnerType { get; set; }

    }
}
