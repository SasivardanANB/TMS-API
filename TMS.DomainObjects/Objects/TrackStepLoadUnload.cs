using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMS.DomainObjects.Objects
{
    public class TrackStepLoadUnload
    {
        public string TrackLoadUnloadName { get; set; }
        public string StepHeaderNotification  { get; set; }
        public TrackStep StartTrip { get; set; }
        public TrackStep ConfirmArrive { get; set; }
        public TrackStep StartLoad { get; set; }
        public TrackStep FinishLoad { get; set; }
    }
}
