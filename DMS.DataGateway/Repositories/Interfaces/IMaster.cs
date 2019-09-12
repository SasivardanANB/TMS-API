using DMS.DomainObjects.Request;
using DMS.DomainObjects.Response;

namespace DMS.DataGateway.Repositories.Interfaces
{
    public interface IMaster
    {
        PartnerResponse CreateUpdatePartner(PartnerRequest partnerRequest);
    }
}
