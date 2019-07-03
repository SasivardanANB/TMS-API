using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMS.DataGateway.DataModels
{
    [Table("Pool", Schema = "TMS")]
    public class Pool : ModelBase
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [MaxLength(15)]
        [Index("Pool_PoolNo", IsUnique = true)]
        public string PoolNo { get; set; }
        [MaxLength(25)]
        [Index("Pool_PoolName", IsUnique = true)]
        public string PoolName { get; set; }
        [MaxLength(50)]
        public string PoolDescription { get; set; }
        [MaxLength(200)]
        public string Address { get; set; }
        [MaxLength(15)]
        public string ContactNumber { get; set; }
        [ForeignKey("City")]
        public int CityID { get; set; }
        public virtual City City { get; set; }
        public bool IsDelete { get; set; }
        [ForeignKey("ImageGuid")]
        public int PhotoId { get; set; }
        public virtual ImageGuid ImageGuid { get; set; }
        
    }
}
