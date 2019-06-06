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
        [MaxLength(50)]
        public string ApplicationCode { get; set; }
        [MaxLength(100)]
        public string ApplicationName { get; set; }
    }
}
