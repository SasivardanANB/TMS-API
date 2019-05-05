using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMS.DataGateway.DataModels
{
    [Table("TripPartnerDetail", Schema = "TMS")]
    public class TripPartnerDetail
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [Index("TripPartnerDetail_TripDetailID", IsUnique = true)]
        public int TripDetailID { get; set; }
        [Index("TripPartnerDetail_PartnerID", IsUnique = true)]
        public int PartnerID { get; set; }
        [Index("TripPartnerDetail_PartnerTypeID", IsUnique = true)]
        public int PartnerTypeID { get; set; }
        public int ParentID { get; set; }
        public int IsParent { get; set; }
        public int IsOriginal { get; set; }
        public int DriverID { get; set; }
        public int VehicleTypeID { get; set; }
        public decimal Price { get; set; }
    }
}
