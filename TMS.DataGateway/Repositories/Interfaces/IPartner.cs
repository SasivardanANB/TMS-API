using TMS.DomainObjects.Request;
using TMS.DomainObjects.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain = TMS.DomainObjects.Objects;

namespace TMS.DataGateway.Repositories.Iterfaces
{
    public interface IPartner
    {
        PartnerResponse CreateUpdatePartner(PartnerRequest partnerRequest);
        PartnerResponse DeletePartner(int partnerId);
        PartnerResponse GetPartners(PartnerRequest partnerRequest);
    }
}
