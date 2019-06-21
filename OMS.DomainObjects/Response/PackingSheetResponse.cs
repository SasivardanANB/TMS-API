using OMS.DomainObjects.Objects;
using System.Collections.Generic;

namespace OMS.DomainObjects.Response
{
    public class PackingSheetResponse : Message
    {
        public List<PackingSheet> Data { get; set; }
    }
}
