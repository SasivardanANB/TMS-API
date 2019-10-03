using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMS.DomainObjects.Objects
{
    public class SubDistrictDeatils
    {
        public int? PostalCodeId { get; set; }
        public string PostalCode { get; set; }
        public int SubDistrictId { get; set; }
        public string SubDistrictName { get; set; }
        public string CityName { get; set; }
        public string ProvinceName { get; set; }

    }
}
