using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMS.DomainObjects.Request;
using TMS.DomainObjects.Response;

namespace TMS.DomainGateway.Task.Interfaces
{
    public interface ITripTask
    {
        TripResponse GetTripList(TripRequest tripRequest);
        OrderDetailsResponse GetTripDetails(int orderId);
        TripResponse UpdateTripDetails(TripRequest tripRequest);
    }
}
