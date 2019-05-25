using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMS.DomainObjects.Objects
{
    public class SAMAUser
    {
        public int ID { get; set; }
        public string UserName { get; set; }
        public string CompanyCode { get; set; }
        public string RegionCode { get; set; }
        public string BranchCode { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string MobilePhone { get; set; }
    }
}
