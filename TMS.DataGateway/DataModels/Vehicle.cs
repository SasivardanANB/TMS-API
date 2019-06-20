using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMS.DataGateway.DataModels
{
    [Table("Vehicle", Schema = "TMS")]
    public class Vehicle : ModelBase
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [MaxLength(12)]
        [Required(ErrorMessage ="Enter valid plate number")]
        public string PlateNumber { get; set; }
        [ForeignKey("VehicleType")]
        public int VehicleTypeID { get; set; }
        public virtual VehicleType VehicleType { get; set; }
        //[MaxLength(20)]
        //public string VehicleTypeName { get; set; }
        [MaxLength(12)]
        public string PoliceNo { get; set; }
        public decimal MaxWeight { get; set; }
        [MaxLength(11)]
        public string MaxDimension { get; set; }
        [MaxLength(25)]
        public string KIRNo { get; set; }
        public DateTime? KIRExpiryDate { get; set; }
        [ForeignKey("Pool")]
        public int PoolID { get; set; }
        public virtual Pool Pool { get; set; }
        public bool IsDedicated { get; set; }
        [ForeignKey("Partner")]
        public int ShipperID { get; set; }
        public virtual Partner Partner { get; set; }
        public bool IsDelete { get; set; }
       
    }
}
