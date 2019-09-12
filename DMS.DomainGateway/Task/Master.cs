using DMS.DomainGateway.Task.Interfaces;
using DMS.DomainObjects.Request;
using DMS.DomainObjects.Response;

namespace DMS.DomainGateway.Task
{
    public abstract class MasterTask : IMasterTask
    {
        public abstract PartnerResponse CreateUpdatePartner(PartnerRequest partnerRequest);
    }
}
