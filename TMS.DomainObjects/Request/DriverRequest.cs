using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMS.DomainObjects.Objects;

namespace TMS.DomainObjects.Request
{
    public class DriverRequest : RequestFilter
    {
        public List<Driver> Requests { get; set; }
    }
}
