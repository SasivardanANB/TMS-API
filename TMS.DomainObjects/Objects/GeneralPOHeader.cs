using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMS.DomainObjects.Objects
{
    public class GeneralPOHeader
    {
        public string GeneralPOHeaderId { get; set; }
        public string DepartementId { get; set; }
        public string OrderDate { get; set; }
        public string OrderNo { get; set; }
        public string LocationId { get; set; }
        public string VendorCode { get; set; }
        public string ReviewerDepartementId { get; set; }
        public string TotalPrice { get; set; }
        public string Currency { get; set; }
        public string Reference { get; set; }
        public string Status { get; set; }
        public string StatusSAP { get; set; }
        public string IsDeleted { get; set; }
        public string CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedDate { get; set; }
        public string ModifiedBy { get; set; }
        public string Note { get; set; }
        public string IsFromSAP { get; set; }
        public string BusinessArea { get; set; }
        public string CompanyCode { get; set; }
        public string GRNumber { get; set; }
        public string GRDate { get; set; }
        public string GRTime { get; set; }

    }
}
