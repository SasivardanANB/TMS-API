using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace OMS.DomainObjects.Objects
{
    public class Application
    {
        public int ID { get; set; }
        public string ApplicationCode { get; set; }
        public string ApplicationName { get; set; }
    }
}
