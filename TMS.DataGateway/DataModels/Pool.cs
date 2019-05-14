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
    public class Pool
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [MaxLength(15)]
        [Index("Pool_PoolNo", IsUnique = true)]
        public string PoolNo { get; set; }
        [MaxLength(10)]
        [Index("Pool_PoolName", IsUnique = true)]
        public string PoolName { get; set; }
        [MaxLength(50)]
        public string PoolDescritpion { get; set; }
        [MaxLength(255)]
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
