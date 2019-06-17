using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DMS.DomainObjects.Objects;

namespace DMS.DomainObjects.Request
{
    public class NotificationRequest
    {
        public string to { get; set; }
        public Notification data { get; set; }
    }
}
