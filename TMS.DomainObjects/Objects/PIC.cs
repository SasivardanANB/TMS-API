using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMS.DomainObjects.Objects
{
    public class PIC
    {
        public long ID { get; set; }
        [MaxLength(30)]
        public string PICName { get; set; }
        [MaxLength(15)]
        public string PICPhone { get; set; }
        [MaxLength(50)]
        public string PICEmail { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        [MaxLength(30)]
        public string PICPassword { get; set; }
        public int PhotoId { get; set; }
        [Required(ErrorMessageResourceType = typeof(Resource.ResourceData), ErrorMessageResourceName = "InvalidImage")]
        public string PhotoGuId { get; set; }
    }
}
