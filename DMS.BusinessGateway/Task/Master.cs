using DMS.DataGateway.Repositories.Interfaces;
using DMS.DomainGateway.Task;
using DMS.DomainObjects.Request;
using DMS.DomainObjects.Response;

namespace DMS.BusinessGateway.Task
{
    public partial class BusinessMasterTask : MasterTask
    {
        private readonly IMaster _masterRepository;
        public BusinessMasterTask(IMaster masterRepository)
        {
            _masterRepository = masterRepository;
        }

        public override PartnerResponse CreateUpdatePartner(PartnerRequest partnerRequest)
        {
            PartnerResponse partnerResponse = _masterRepository.CreateUpdatePartner(partnerRequest);
            return partnerResponse;
        }
    }
}
