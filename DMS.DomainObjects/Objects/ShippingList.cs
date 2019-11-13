using Newtonsoft.Json;
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
        [JsonProperty(PropertyName = "PKGList")]
        public List<String> PKG_List { get; set; }
        [JsonProperty(PropertyName = "SLNo")]
        public string SL_No { get; set; }
    }
}
