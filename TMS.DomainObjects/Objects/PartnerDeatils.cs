using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMS.DomainObjects.Objects
{
    public class PartnerDeatils
    {
        public int PostalCodeId { get; set; }
        public string PostalCode { get; set; }
        public int SubDistrictId { get; set; }
        public string SubDistrictName { get; set; }
        public int CityId { get; set; }
        public string CityName { get; set; }
        public int ProvinceId { get; set; }
        public string ProvinceName { get; set; }
        public string Address { get; set; }

    }
}
