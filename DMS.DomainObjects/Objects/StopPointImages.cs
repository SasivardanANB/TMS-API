using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMS.DomainObjects.Objects
{
    public class StopPointImages
    {
        public int ID { get; set; }
        public int StopPointId { get; set; }
        public string ImageGUId { get; set; }
        public string ImageType { get; set; }        
    }
}
