using OMS.DomainObjects.Request;
using OMS.DomainObjects.Response;

namespace OMS.DomainGateway.Task.Interfaces
{
    public interface IMasterTask
    {
        PartnerResponse CreateUpdatePartner(PartnerRequest partnerRequest);
    }
}
