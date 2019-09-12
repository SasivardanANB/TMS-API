using OMS.DomainGateway.Task.Interfaces;
using OMS.DomainObjects.Request;
using OMS.DomainObjects.Response;

namespace OMS.DomainGateway.Task
{
    public abstract class MasterTask : IMasterTask
    {
        public abstract PartnerResponse CreateUpdatePartner(PartnerRequest partnerRequest);
    }
}
