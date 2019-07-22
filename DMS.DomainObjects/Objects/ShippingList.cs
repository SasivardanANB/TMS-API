using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMS.DomainObjects.Objects
{
    public class ShippingList
    {
        public string Colie { get; set; } 
        public List<String> PKG_List { get; set; }
        public string SL_No { get; set; }
    }
}
