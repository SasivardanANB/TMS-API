using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMS.DomainObjects.Objects
{
    public class RequestFilter
    {
        public string CreatedBy { get; set; }
        public DateTime CreatedTime { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime LastModifiedTime { get; set; }
        public string SortOrder { get; set; }
        public int? PageSize { get; set; }
        public int? PageNumber { get; set; }
        public string GlobalSearch { get; set; }
    }
}
