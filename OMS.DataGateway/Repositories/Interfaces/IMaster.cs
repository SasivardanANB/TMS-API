using OMS.DomainObjects.Request;
using OMS.DomainObjects.Response;

namespace OMS.DataGateway.Repositories.Interfaces
{
    public interface IMaster
    {
        PartnerResponse CreateUpdatePartner(PartnerRequest partnerRequest);
    }
}
