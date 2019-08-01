using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMS.DomainObjects.Objects
{
    public class ResponseDataForFileUpload
    {
        public string Status { get; set; }
        public int StatusCode { get; set; }
        public string StatusMessage { get; set; }
        public string Guid { get; set; }
    }
}
