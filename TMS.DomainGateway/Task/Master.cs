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
        public abstract PartnerDetilasResponse GetPartnersDetails(int partnerId);
        public abstract CommonResponse GetDriverNames();
        public abstract CommonResponse GetVehicleTypeNames();
        public abstract CommonResponse GetFleetTypeNames();
        public abstract SubDistrictDetailsResponse GetSubDistrictDetails(string searchText);
        public abstract CommonResponse GetPoolNames(string searchText);
        public abstract CommonResponse GetShipperNames(string searchText);
        public abstract CommonResponse GetCityNames(string searchText);
        public abstract CommonResponse GetGateNamesByBusinessArea(int businessAreaId, int gateTypeId);
        public abstract CommonResponse GetTripStatusNames();
    }
}
