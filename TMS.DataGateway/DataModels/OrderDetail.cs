using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMS.DataGateway.DataModels
{
    [Table("OrderDetail", Schema = "TMS")]
    public class OrderDetail
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [Index("OrderDetail_OrderHeaderID", IsUnique = true)]
        public int OrderHeaderID { get; set; }
        [Index("OrderDetail_ItemNo", IsUnique = true)]
        public int ItemNo { get; set; }
        public string Pengirim { get; set; }
        public string Penerima { get; set; }
        public string Instruksi { get; set; }
        public DateTime EstimatedArrivalTime { get; set; }
        public DateTime ActualArrivalTime { get; set; }
    }
}
