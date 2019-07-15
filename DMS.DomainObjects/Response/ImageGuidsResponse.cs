using DMS.DomainObjects.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMS.DomainObjects.Response
{
    public class ImageGuidsResponse : Message
    {
        public List<ImageGuid> Data { get; set; }
    }
}
