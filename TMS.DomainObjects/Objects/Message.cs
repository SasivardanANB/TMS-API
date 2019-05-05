using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMS.DomainObjects.Objects
{
    public class Message
    {
        public string Status { get; set; }
        public int StatusCode { get; set; }
        public string StatusMessage { get; set; }
        public string TokenKey { get; set; }
        public DateTime TokenIssuedOn { get; set; }
        public DateTime TokenExpiresOn { get; set; }
        public int NumberOfRecords { get; set; }
    }
}
