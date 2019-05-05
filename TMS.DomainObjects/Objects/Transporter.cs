using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMS.DomainObjects.Objects
{
    public class Transporter
    {
        public int ID { get; set; }
        public string Initial { get; set; }
        [Required(ErrorMessageResourceType = typeof(Resource.ResourceData), ErrorMessageResourceName = "InvalidExpeditorName")]
        public string ExpeditorName { get; set; }
        [Required(ErrorMessageResourceType = typeof(Resource.ResourceData), ErrorMessageResourceName = "InvalidExpeditorEmail")]
        [EmailAddress(ErrorMessageResourceType = typeof(Resource.ResourceData),ErrorMessageResourceName = "ExpeditorEmailCheck")]
        public string ExpeditorEmail { get; set; }
        public string Address { get; set; }
        [Range(minimum:1,maximum: int.MaxValue,ErrorMessageResourceType = typeof(Resource.ResourceData),ErrorMessageResourceName = "InvalidPostalCodeID")]
        public int PostalCodeID { get; set; }
        [Range(minimum: 1, maximum: int.MaxValue, ErrorMessageResourceType = typeof(Resource.ResourceData), ErrorMessageResourceName = "InvalidPICID")]
        public int PICID { get; set; }
        public bool TypeCode { get; set; }
        [Range(minimum: 1, maximum: int.MaxValue, ErrorMessageResourceType = typeof(Resource.ResourceData), ErrorMessageResourceName = "InvalidExpeditorTypeID")]
        public int ExpeditorTypeID { get; set; }
    }
}
