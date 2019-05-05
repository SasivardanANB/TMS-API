using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMS.DomainObjects.Objects
{
    public class SubDistrict
    {
        public int ID { get; set; }
        public string SubdistrictCode { get; set; }
        public string SubdistrictName { get; set; }
        //public City City { get; set; }
        public int CityID { get; set; }
    }
}
