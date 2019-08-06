using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMS.DomainObjects.Objects
{
    public class Invoice
    {
        public GeneralPOHeader GeneralPOHeader { get; set; }
        public GeneralPODetails GeneralPODetails { get; set; }
    }
}
