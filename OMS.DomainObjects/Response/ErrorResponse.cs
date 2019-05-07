using OMS.DomainObjects.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMS.DomainObjects.Response
{
    public class ErrorResponse : Message
    {
        public List<Error> Data { get; set; }
    }
}
