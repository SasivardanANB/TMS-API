using DMS.DataGateway.Repositories;
using DMS.DataGateway.Repositories.Iterfaces;
using DMS.DomainGateway.Task;
using DMS.DomainObjects.Request;
using DMS.DomainObjects.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMS.BusinessGateway.Task
{
    public partial class BusinessTripTask : TripTask
    {
        private ITrip _tripRepository;

        public BusinessTripTask(ITrip tripRepository)
        {
            _tripRepository = tripRepository;
        }

        public override StopPointOrderItemsResponse GetOrderItemsByStopPoint(StopPointsRequest stopPointsByTripRequest)
        {
            StopPointOrderItemsResponse tripData = _tripRepository.GetOrderItemsByStopPoint(stopPointsByTripRequest);
            return tripData;
        }

        public override StopPointsResponse GetStopPointsByTrip(StopPointsRequest stopPointsByTripRequest)
        {
            StopPointsResponse tripData = _tripRepository.GetStopPointsByTrip(stopPointsByTripRequest);
            return tripData;
        }

        public override TripResponse GetTripsByDriver(TripsByDriverRequest tripsByDriverRequest)
        {
            TripResponse tripData = _tripRepository.GetTripsByDriver(tripsByDriverRequest);
            return tripData;
        }

        public override UpdateTripStatusResponse UpdateTripStatusEventLog(UpdateTripStatusRequest updateTripStatusRequest)
        {
            UpdateTripStatusResponse tripData = _tripRepository.UpdateTripStatusEventLog(updateTripStatusRequest);
            return tripData;
        }

        public override TripResponse UpdateEntireTripStatus(TripsByDriverRequest tripsByDriverRequest)
        {
            TripResponse tripData = _tripRepository.UpdateEntireTripStatus(tripsByDriverRequest);
            return tripData;
        }

        public override TripResponse CreateUpdateTrip(TripRequest request)
        {
            TripResponse tripData = _tripRepository.CreateUpdateTrip(request);
            return tripData;
        }

        public override string GetOrderNumber(int stopPointId)
        {
            return _tripRepository.GetOrderNumber(stopPointId);
        }

        public override string GetOrderStatusCode(int tripStatusId)
        {
            return _tripRepository.GetOrderStatusCode(tripStatusId);
        }

        public override int GetOrderSequnceNumber(int stopPointId)
        {
            return _tripRepository.GetOrderSequnceNumber(stopPointId);
        }
        public override StopPointsResponse GetLastTripStatusData(int stopPointId)
        {
            return _tripRepository.GetLastTripStatusData(stopPointId);
        }
        public override string GetDeviceId(string token)
        {
            return _tripRepository.GetDeviceId(token);
        }
    }
}
