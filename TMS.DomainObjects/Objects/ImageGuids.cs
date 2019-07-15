using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMS.DomainObjects.Objects
{
    public class ImageGuids
    {
        public string Guid { get; set; }
        public string ImageName { get; set; }
        public int SequenceNo { get; set; }
    }
}
