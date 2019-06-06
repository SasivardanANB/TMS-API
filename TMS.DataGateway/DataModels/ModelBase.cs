using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMS.DataGateway.DataModels
{
   public abstract class ModelBase
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        [MaxLength(100)]
        public string CreatedBy
        {
            get { return "SYSTEM"; }
            set { }
        }
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime CreatedTime { get; set; }
        [MaxLength(100)]
        public string LastModifiedBy { get; set; }
        public DateTime? LastModifiedTime { get; set; }
    }
}
