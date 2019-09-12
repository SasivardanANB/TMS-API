using DMS.DomainObjects.Objects;
using System.Collections.Generic;

namespace DMS.DomainObjects.Request
{
    public class PartnerRequest : RequestFilter
    {
        public List<Partner> Requests { get; set; }
    }
}
