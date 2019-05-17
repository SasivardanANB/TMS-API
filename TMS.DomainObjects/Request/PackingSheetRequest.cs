using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMS.DomainObjects.Objects;

namespace TMS.DomainObjects.Request
{
    public class PackingSheetRequest : RequestFilter
    {
        public List<PackingSheet> Requests { get; set; }
    }
}
