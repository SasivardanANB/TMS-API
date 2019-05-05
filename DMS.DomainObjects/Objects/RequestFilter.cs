using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMS.DomainObjects.Objects
{
    public class RequestFilter
    {
        public string SortOrder { get; set; }
        public int? Page { get; set; }
    }
}
