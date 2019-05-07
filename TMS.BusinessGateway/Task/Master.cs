using TMS.DataGateway.Repositories;
using TMS.DataGateway.Repositories.Iterfaces;
using TMS.DomainGateway.Task;
using TMS.DomainObjects.Request;
using TMS.DomainObjects.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMS.BusinessGateway.Task
{
    public partial class BusinessMasterTask : MasterTask
    {
        private readonly IMaster _masterRepository;
        public BusinessMasterTask(IMaster masterRepository)
        {
            _masterRepository = masterRepository;
        }

        public override PartnerSearchResponse GetPartners(PartnerSearchRequest partnerSearchRequest)
        {
            PartnerSearchResponse partnerSearchResponse = _masterRepository.GetPartners(partnerSearchRequest);
            return partnerSearchResponse;
        }

        public override CommonResponse GetDriverNames()
        {
            CommonResponse commonResponse = _masterRepository.GetDriverNames();
            return commonResponse;
        }

        public override CommonResponse GetVehicleTypeNames()
        {
            CommonResponse commonResponse = _masterRepository.GetVehicleTypeNames();
            return commonResponse;
        }

    }
}
