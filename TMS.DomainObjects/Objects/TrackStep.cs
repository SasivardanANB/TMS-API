using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMS.DomainObjects.Objects
{
    public class TrackStep
    {
        public string StepHeaderName { get; set; }
        public string StepHeaderDescription { get; set; }
        public string StepHeaderDateTime { get; set; }
        public string StepHeaderNotification { get; set; }
    }
}
