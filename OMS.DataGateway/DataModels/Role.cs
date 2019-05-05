using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMS.DataGateway.DataModels
{
    [Table("Role", Schema = "OMS")]
    public class Role
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [Required(ErrorMessage = "")]
        [MaxLength(4)]
        [Index("Role_RoleCode", IsUnique = true)]
        public string RoleCode { get; set; }
        [MaxLength(30)]
        public string RoleDescription { get; set; }
        [DataType(DataType.Date,ErrorMessage ="Enter valid date")]
        public DateTime ValidFrom { get; set; }
        [DataType(DataType.Date, ErrorMessage = "Enter valid date")]
        public DateTime ValidTo { get; set; }
        public bool IsActive { get; set; }
        public bool IsDelete { get; set; }
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
