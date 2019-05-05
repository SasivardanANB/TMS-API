using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMS.DataGateway.Repositories.Interfaces;
using TMS.DataGateway.Repositories.Iterfaces;
using TMS.DomainGateway.Task;
using TMS.DomainObjects.Request;
using TMS.DomainObjects.Response;

namespace TMS.BusinessGateway.Task
{
    public partial class BusinessPartnerTask : PartnerTask
    {
        private readonly IPartner _partnerRepository;

        public BusinessPartnerTask(IPartner partnerRepository)
        {
            _partnerRepository = partnerRepository;
        }

        public override PartnerResponse CreateUpdatePartner(PartnerRequest partnerRequest)
        {
            PartnerResponse partnerResponse = _partnerRepository.CreateUpdatePartner(partnerRequest);
            return partnerResponse;
        }

        public override PartnerResponse DeletePartner(int partnerId)
        {
            PartnerResponse partnerResponse = _partnerRepository.DeletePartner(partnerId);
            return partnerResponse;
        }

        public override PartnerResponse GetPartners(PartnerRequest partnerRequest)
        {
            PartnerResponse partnerResponse = _partnerRepository.GetPartners(partnerRequest);
            return partnerResponse;
        }
    }
}
