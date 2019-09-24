using OMS.DomainObjects.Objects;
using System.Collections.Generic;

namespace OMS.DomainObjects.Request
{
    public class PartnerRequest : RequestFilter
    {
        public List<Partner> Requests { get; set; }
    }
}
