using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMS.DomainObjects.Objects
{
    public class Gate
    {
        public int ID { get; set; }
        public int OrderId { get; set; }
        //[Required(ErrorMessageResourceType = typeof(Resource.ResourceData), ErrorMessageResourceName = "")]
        public string GateTypeDescription { get; set; }
        //[Required(ErrorMessageResourceType = typeof(Resource.ResourceData), ErrorMessageResourceName = "")]
        //[Range(minimum: 1, maximum: 2, ErrorMessageResourceType = typeof(Resource.ResourceData), ErrorMessageResourceName = "")]
        public int GateTypeId { get; set; }
        [MaxLength(12)]
        public string VehicleNumber { get; set; }
        public string OrderType { get; set; }
        public string VehicleTypeName { get; set; }
        public string Status { get; set; }
        public int GateId { get; set; }
        public string GateName { get; set; }
        [MaxLength(120)]
        public string Info { get; set; }
        public int BusinessAreaId { get; set; }
        public string BusinessArea { get; set; }
    }
}
