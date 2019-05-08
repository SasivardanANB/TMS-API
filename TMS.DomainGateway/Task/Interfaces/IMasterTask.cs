using TMS.DomainObjects.Request;
using TMS.DomainObjects.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMS.DomainGateway.Task.Interfaces
{
    public interface IMasterTask
    {
        PartnerSearchResponse GetPartners(PartnerSearchRequest partnerSearchRequest);
        PartnerResponse GetPartnersDetails(int partnerId);
        CommonResponse GetDriverNames();
        CommonResponse GetVehicleTypeNames();
        CommonResponse GetFleetTypeNames();
    }
}
