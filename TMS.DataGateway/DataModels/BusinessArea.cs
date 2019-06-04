using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMS.DataGateway.DataModels
{
    [Table("BusinessArea", Schema = "TMS")]
    public class BusinessArea
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [Required(ErrorMessage = "")]
        [MaxLength(4)]
        [Index("BusinessArea_BusinessAreaCode", IsUnique = true)]
        public string BusinessAreaCode { get; set; }
        [MaxLength(100)]
        public string BusinessAreaDescription { get; set; }
        [ForeignKey("CompanyCode")]
        public int? CompanyCodeID { get; set; }
        public CompanyCode CompanyCode { get; set; }
        [MaxLength(200)]
        public string Address { get; set; }
        [ForeignKey("PostalCode")]
        public int? PostalCodeID { get; set; }
        public PostalCode PostalCode { get; set; }
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
