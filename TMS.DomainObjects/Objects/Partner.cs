using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMS.DomainObjects.Objects
{
    public class Partner
    {
        public int ID { get; set; }
        public Nullable<int> OrderPointTypeID { get; set; }
        public string OrderPointCode { get; set; }
        public string PartnerNo { get; set; }
        public string PartnerInitial { get; set; }
        [Required(ErrorMessageResourceType = typeof(Resource.ResourceData), ErrorMessageResourceName = "InvalidExpeditorName")]
        public string PartnerName { get; set; }
        [Required(ErrorMessageResourceType = typeof(Resource.ResourceData), ErrorMessageResourceName = "InvalidExpeditorEmail")]
        [EmailAddress(ErrorMessageResourceType = typeof(Resource.ResourceData), ErrorMessageResourceName = "ExpeditorEmailCheck")]
        public string PartnerEmail { get; set; }
        public string PartnerAddress { get; set; }
        [Range(minimum: 1, maximum: int.MaxValue, ErrorMessageResourceType = typeof(Resource.ResourceData), ErrorMessageResourceName = "InvalidPostalCodeID")]
        public int? PostalCodeID { get; set; }
        [Range(minimum: 1, maximum: int.MaxValue, ErrorMessageResourceType = typeof(Resource.ResourceData), ErrorMessageResourceName = "InvalidPICID")]
        public int? PICID { get; set; }
        public string PICName { get; set; }
        [Range(minimum: 1, maximum: 4, ErrorMessageResourceType = typeof(Resource.ResourceData), ErrorMessageResourceName = "InvalidExpeditorTypeID")]
        public int PartnerTypeID { get; set; }
        public bool IsDeleted { get; set; }
        public string CityCode { get; set; }
        public string ProvinceCode { get; set; }

    }
}
