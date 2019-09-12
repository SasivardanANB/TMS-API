using DMS.DomainObjects.Request;
using DMS.DomainObjects.Response;

namespace DMS.DomainGateway.Task.Interfaces
{
    public interface IMasterTask
    {
        PartnerResponse CreateUpdatePartner(PartnerRequest partnerRequest);
    }
}
