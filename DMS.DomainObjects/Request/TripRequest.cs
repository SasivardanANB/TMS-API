using DMS.DomainObjects.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMS.DomainObjects.Request
{
    public class TripRequest:RequestFilter 
    {
        public List<Trip> Requests { get; set; }
    }
}
