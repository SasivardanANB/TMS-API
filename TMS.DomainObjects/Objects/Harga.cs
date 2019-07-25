using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMS.DomainObjects.Objects
{
    public class Harga
    {
        public int ID { get; set; }
        public int TransporterID { get; set; }
        public int VechicleTypeID { get; set; }
        public decimal Price { get; set; }
    }
}
