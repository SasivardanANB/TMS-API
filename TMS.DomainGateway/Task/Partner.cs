using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMS.DomainGateway.Task.Interfaces;
using TMS.DomainObjects.Request;
using TMS.DomainObjects.Response;

namespace TMS.DomainGateway.Task
{
    public abstract class PartnerTask : IPartnerTask
    {
        public abstract PartnerResponse CreateUpdatePartner(PartnerRequest partnerRequest);
        public abstract PartnerResponse DeletePartner(int partnerId);
        public abstract PartnerResponse GetPartners(PartnerRequest partnerRequest);
    }
}
