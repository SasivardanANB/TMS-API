using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMS.DataGateway.Repositories.Interfaces;
using TMS.DataGateway.Repositories.Iterfaces;
using TMS.DomainGateway.Task;
using TMS.DomainObjects.Request;
using TMS.DomainObjects.Response;

namespace TMS.BusinessGateway.Task
{
    public partial class BusinessTripTask : TripTask
    {
        private readonly ITrip _tripRepository;

        public BusinessTripTask(ITrip tripRepository)
        {
            _tripRepository = tripRepository;
        }

        public override TripResponse GetTripList(TripRequest tripRequest)
        {
            TripResponse tripResponse = _tripRepository.GetTripList(tripRequest);
            return tripResponse;
        }
    }
}
