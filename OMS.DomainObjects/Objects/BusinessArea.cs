using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMS.DomainObjects.Objects
{
    public class BusinessArea
    {
        public int ID { get; set; }
        public string BusinessAreaCode { get; set; }
        public string BusinessAreaDescription { get; set; }
        public int CompanyCodeID { get; set; }
        public string Address { get; set; }
        public int PostalCodeID { get; set; }
    }
}
