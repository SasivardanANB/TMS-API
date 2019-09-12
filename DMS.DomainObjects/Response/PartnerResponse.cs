using DMS.DomainObjects.Objects;
using System.Collections.Generic;

namespace DMS.DomainObjects.Response
{
    public class PartnerResponse : Message
    {
        public List<Partner> Data { get; set; }
    }
}
