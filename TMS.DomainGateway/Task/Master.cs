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
    public abstract class MasterTask : IMasterTask
    {
        public abstract PartnerSearchResponse GetPartners(PartnerSearchRequest partnerSearchRequest);
        public abstract PartnerResponse GetPartnersDetails(int partnerId);
        public abstract CommonResponse GetDriverNames();
        public abstract CommonResponse GetVehicleTypeNames();
        public abstract CommonResponse GetFleetTypeNames();
        public abstract SubDistrictDetailsResponse GetSubDistrictDetails(string searchText);
    }
}
