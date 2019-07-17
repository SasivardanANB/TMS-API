using System.Collections.Generic;
using TMS.DomainObjects.Objects;

namespace TMS.DomainObjects.Response
{
    public class HargaResponse : Message
    {
        public List<Harga> Data { get; set; }
    }
}
