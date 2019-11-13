using DMS.DomainGateway.Task.Interfaces;
using DMS.DomainObjects.Objects;
using DMS.DomainObjects.Request;
using DMS.DomainObjects.Response;

namespace DMS.DomainGateway.Task
{
    public abstract class TripTask : ITripTask
    {
        public abstract StopPointOrderItemsResponse GetOrderItemsByStopPoint(StopPointsRequest stopPointsByTripRequest);
        public abstract StopPointsResponse GetStopPointsByTrip(StopPointsRequest stopPointsByTripRequest);
        public abstract TripResponse GetTripsByDriver(TripsByDriverRequest tripsByDriverRequest);
        public abstract UpdateTripStatusResponse UpdateTripStatusEventLog(UpdateTripStatusRequest updateTripStatusRequest);
        public abstract TripResponse UpdateEntireTripStatus(TripsByDriverRequest tripsByDriverRequest);

        public abstract TripResponse CreateUpdateTrip(TripRequest request);
        public abstract string GetOrderNumber(int stopPointId);
        public abstract string GetOrderStatusCode(int tripStatusId);
        public abstract int GetOrderSequnceNumber(int stopPointId);
        public abstract StopPointsResponse GetLastTripStatusData(int stopPointId);
        public abstract string GetDeviceId(string token);
        public abstract TripResponse ReAssignTrip(TripRequest tripRequest);
        public abstract ImageGuidsResponse GetShippingListGuids(string orderNumber);
        public abstract ImageGuidsResponse GetPodGuids(string orderNumber);
        public abstract ImageGuidsResponse GetPhotoWithCustomerGuids(string orderNumber);
        public abstract StopPointsResponse GetPendingStopPoints(int tripId);

        public abstract OrderStatusResponse CancelOrder(OrderStatusRequest request);

        public abstract ShippingList CreateUpdateShipmentList(int stopPointId, ShippingList request);
        public abstract StopPointsResponse SwapeStopPoints(UpdateTripStatusRequest updateTripStatusRequest);
    }
}
