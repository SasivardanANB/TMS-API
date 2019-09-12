using OMS.DataGateway.Repositories.Interfaces;
using OMS.DomainGateway.Task;
using OMS.DomainObjects.Request;
using OMS.DomainObjects.Response;

namespace OMS.BusinessGateway.Task
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
