using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
//using CsvHelper.Configuration.Attributes;

namespace DMS.DataGateway.DataModels
{
    [Table("Province", Schema = "DMS")]
    public class Province
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [MaxLength(4)]
        [Index("Province_ProvinceCode", IsUnique = true)]
        public string ProvinceCode { get; set; }
        [MaxLength(50)]
        public string ProvinceDescription { get; set; }
        public string CreatedBy
        {
            get;
            set;
        }
        public DateTime? CreatedTime
        {
            get;
            set;
        }
        public string LastModifiedBy { get; set; }
        public DateTime? LastModifiedTime { get; set; }
    }
}
