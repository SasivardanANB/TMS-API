﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMS.DomainObjects.Objects
{
    public class TrackHeader
    {
        public int StepHeaderNumber { get; set; }
        public string StepHeaderName { get; set; }
        public string StepHeaderNotification { get; set; }
        public List<TrackDetail> TackDetails { get; set; }
    }
}
