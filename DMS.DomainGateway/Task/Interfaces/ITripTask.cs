using DMS.DomainObjects.Request;
using DMS.DomainObjects.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMS.DomainGateway.Task.Interfaces
{
    public interface ITripTask
    {
        TripResponse GetTripsByDriver(TripsByDriverRequest tripsByDriverRequest);
        StopPointsResponse GetStopPointsByTrip(StopPointsRequest stopPointsByTripRequest);
        StopPointOrderItemsResponse GetOrderItemsByStopPoint(StopPointsRequest stopPointsByTripRequest);
        UpdateTripStatusResponse UpdateTripStatusEventLog(UpdateTripStatusRequest updateTripStatusRequest);
        TripResponse UpdateEntireTripStatus(TripsByDriverRequest tripsByDriverRequest);
        TripResponse CreateUpdateTrip(TripRequest request);
        string GetOrderNumber(int stopPointId);
        string GetOrderStatusCode(int tripStatusId);
        int GetOrderSequnceNumber(int stopPointId);
        StopPointsResponse GetLastTripStatusData(int stopPointId);
        string GetDeviceId(string token);
        TripResponse ReAssignTrip(TripRequest tripRequest);
        ImageGuidsResponse GetShippingListGuids(string orderNumber);
        ImageGuidsResponse GetPodGuids(string orderNumber);
        ImageGuidsResponse GetPhotoWithCustomerGuids(string orderNumber);
        StopPointsResponse GetPendingStopPoints(int tripId);
        OrderStatusResponse CancelOrder(OrderStatusRequest request);
    }
}
