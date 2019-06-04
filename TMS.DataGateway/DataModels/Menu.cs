using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMS.DataGateway.DataModels
{
    [Table("Menu", Schema = "TMS")]
    public class Menu : ModelBase
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [Required()]
        [MaxLength(5)]
        [Index("Menu_MenuCode", IsUnique = true)]
        public string MenuCode { get; set; }
        [MaxLength(50)]
        public string MenuDescription { get; set; }
        [MaxLength(100)]
        public string MenuURL { get; set; }
       
    }
}
