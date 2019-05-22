using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMS.DomainObjects.Objects
{
    public class TrackHeader
    {
        public TrackStep AcceptOrder { get; set; }

        public List<TrackStepLoadUnload> Loads { get; set; }
        public List<TrackStepLoadUnload> Unloads { get; set; }
        public TrackStep POD { get; set; }
        public TrackStep Complete { get; set; }
    }
}
