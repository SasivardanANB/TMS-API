using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMS.DomainObjects.Request;
using TMS.DomainObjects.Response;

namespace TMS.DomainGateway.Task.Interfaces
{
    public interface IPartnerTask
    {
        PartnerResponse CreateUpdatePartner(PartnerRequest partnerRequest);
        PartnerResponse DeletePartner(int partnerId);
        PartnerResponse GetPartners(PartnerRequest partnerRequest);
    }
}
