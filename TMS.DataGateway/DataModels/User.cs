using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMS.DataGateway.DataModels
{
    [Table("User", Schema = "TMS")]
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [Required(ErrorMessage = "")]
        [MaxLength(50)]
        public string UserName { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
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
