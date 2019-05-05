using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMS.DomainObjects.Objects
{
    public class SubDisctrict
    {
        public int ID { get; set; }
        public string SubDistrictCode { get; set; }
        public string SubDistrictName { get; set; }
        public int CityID { get; set; }
    }
}
