using TMS.DomainObjects.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMS.DomainObjects.Response
{
    public class TransporterResponse : Message
    {
        public List<Transporter> Data { get; set; }
    }
}
