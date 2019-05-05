using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMS.DataGateway.DataModels
{
    [Table("TripDetail", Schema = "TMS")]
    public class TripDetail
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [Index("TripDetail_TripHeaderID", IsUnique = true)]
        public int TripHeaderID { get; set; }
        [Index("TripDetail_ItemNo", IsUnique = true)]
        public int ItemNo { get; set; }
        public int OrderDetailID { get; set; }
        public decimal Berat { get; set; }
        public decimal KM { get; set; }
        public decimal Price { get; set; }
    }
}
