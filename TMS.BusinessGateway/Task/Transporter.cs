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
    public partial class BusinessTransporterTask : TransporterTask
    {
        private readonly ITransporter _userRepository;

        public BusinessTransporterTask(ITransporter userRepository)
        {
            _userRepository = userRepository;
        }

        public override TransporterResponse CreateUpdateTransporter(TransporterRequest transporterRequest)
        {
            TransporterResponse transporterResponse = _userRepository.CreateUpdateTransporter(transporterRequest);
            return transporterResponse;
        }

        public override TransporterResponse DeleteTransporter(int transporterId)
        {
            TransporterResponse transporterResponse = _userRepository.DeleteTransporter(transporterId);
            return transporterResponse;
        }

        public override TransporterResponse GetTransporters(TransporterRequest transporterRequest)
        {
            TransporterResponse transporterResponse = _userRepository.GetTransporters(transporterRequest);
            return transporterResponse;
        }
    }
}
