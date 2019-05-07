using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMS.DataGateway.DataModels
{
    [Table("Partner", Schema = "TMS")]
    public class Partner
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public Nullable<int> OrderPointTypeID { get; set; }
        [MaxLength(10)]
        //[Index("Partner_OrderPointCode", IsUnique = true)]
        public string OrderPointCode { get; set; }
        [MaxLength(10)]
        [Index("Partner_PartnerNo", IsUnique = true)]
        public string PartnerNo { get; set; }
        public string PartnerName { get; set; }
        public string PartnerAddress { get; set; }
        [ForeignKey("PostalCode")]
        public int? PostalCodeID { get; set; }
        public virtual PostalCode PostalCode { get; set; }
        public int PartnerTypeID { get; set; }
        public string PartnerInitial { get; set; }
        public string PartnerEmail { get; set; }
        [ForeignKey("PIC")]
        public int? PICID { get; set; }
        public virtual PIC PIC { get; set; }
        public bool IsDeleted { get; set; }
        public PartnerType PartnerType { get; set; }
        public bool IsActive { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public string CreatedBy
        {
            get { return "SYSTEM"; }
            set { }
        }
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime CreatedTime
        {
            get { return DateTime.Now; }
            set { }
        }
        public string LastModifiedBy { get; set; }
        public DateTime? LastModifiedTime { get; set; }

    }
}
