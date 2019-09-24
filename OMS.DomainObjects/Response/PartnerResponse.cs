using OMS.DomainObjects.Objects;
using System.Collections.Generic;

namespace OMS.DomainObjects.Response
{
    public class PartnerResponse : Message
    {
        public List<Partner> Data { get; set; }
    }
}
