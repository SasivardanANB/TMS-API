using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMS.DomainObjects.Objects
{
    public class City
    {
        public int ID { get; set; }
        public string CityCode { get; set; }
        public string CityDescription { get; set; }
        public int ProvinceID { get; set; }
    }
}
