using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMS.DomainObjects.Objects
{
    public class PackingSheet
    {
        public string OrderNumber { get; set; }
        public string Notes { get; set; }
        public int DealerId { get; set; }
        public int OrderDetailId { get; set; }
        public string ShippingListNo { get; set; }
        public int Collie { get; set; }
        public string Katerangan { get; set; }

        public List<Common> PackingSheetNumbers { get; set; } 

    }
}
