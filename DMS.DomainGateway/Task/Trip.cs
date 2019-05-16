using DMS.DomainGateway.Task.Interfaces;
using DMS.DomainObjects.Request;
using DMS.DomainObjects.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMS.DomainGateway.Task
{
    public abstract class TripTask : ITripTask
    {
        public abstract StopPointOrderItemsResponse GetOrderItemsByStopPoint(StopPointsRequest stopPointsByTripRequest);
        public abstract StopPointsResponse GetStopPointsByTrip(StopPointsRequest stopPointsByTripRequest);
        public abstract TripResponse GetTripsByDriver(TripsByDriverRequest tripsByDriverRequest);
        public abstract UpdateTripStatusResponse UpdateTripStatusEventLog(UpdateTripStatusRequest updateTripStatusRequest);
        public abstract TripResponse UpdateEntireTripStatus(TripsByDriverRequest tripsByDriverRequest);
    }
}
