using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMS.DomainObjects.Objects
{
    public class Location
    {
        public int ID { get; set; }
        public string TypeofLocation { get; set; }
        public string Place { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public int CityId { get; set; }
    }
}
