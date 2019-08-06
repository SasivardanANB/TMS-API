using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMS.DomainObjects.Objects
{
    public class GeneralPODetails
    {
        public string GRNumber { get; set; }
        public string MaterialNumber { get; set; }
        public string GeneralPODetailId { get; set; }
        public string GeneralPOHeaderId { get; set; }
        public string OrderDescription { get; set; }
        public string Qty { get; set; }
        public string DeliveryDate { get; set; }
        public string UnitPrice { get; set; }
        public string PPN { get; set; }
        public string TotalPrice { get; set; }
        public string Jenis { get; set; }
        public string DepartementId { get; set; }
        public string Currency { get; set; }
        public string ItemNo { get; set; }
        public string IsDeleted { get; set; }
        public string CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedDate { get; set; }
        public string ModifiedBy { get; set; }
        public string MaterialDesc { get; set; }
    }
}
