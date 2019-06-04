using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMS.DomainObjects.Objects
{
    public class Region
    {
        public int ID { get; set; }
        [MaxLength(4)]
        public string BusinessAreaCode { get; set; }
        [MaxLength(100)]
        public string BusinessAreaDescription { get; set; }
        //public CompanyCode CompanyCode { get; set; }
        public int CompanyCodeID { get; set; }
        [MaxLength(200)]
        public string Address { get; set; }
        //public PostalCode PostalCode { get; set; }
        public int PostalCodeID { get; set; }
    }
}
