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
    public interface IMaster
    {
        PartnerSearchResponse GetPartners(PartnerSearchRequest partnerSearchRequest);
        PartnerDetilasResponse GetPartnersDetails(int partnerId);
        CommonResponse GetDriverNames();
        CommonResponse GetVehicleTypeNames();
        CommonResponse GetFleetTypeNames();
        SubDistrictDetailsResponse GetSubDistrictDetails(string searchText);
        CommonResponse GetPoolNames(string searchText);
        CommonResponse GetShipperNames(string searchText);
        CommonResponse GetCityNames(string searchText);
        CommonResponse GetGateNamesByBusinessArea(int businessAreaId, int gateTypeId);
        CommonResponse GetTripStatusNames();
    }
}
