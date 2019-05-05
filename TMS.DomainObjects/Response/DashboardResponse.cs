using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMS.DomainObjects.Objects;

namespace TMS.DomainObjects.Response
{
    public class DashboardResponse : Message
    {
        public int AllOrderCount { get; set; }
        public int BookedCount { get; set; }
        public int PickUpCount { get; set; }
        public int LoadingCount { get; set; }
        public int ConfirmedCount { get; set; }
        public int Acceptedcount { get; set; }
        public int UnloadingCount { get; set; }
        public int DropOffCount { get; set; }
        public int PODCount { get; set; }
        public int CancelledCount { get; set; }
    }
}
