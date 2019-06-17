using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMS.DomainObjects.Objects
{
    public class Notification
    {
        public string title { get; set; }
        public string message { get; set; }
        public int badgecount { get; set; }
        public string click_action { get; set; }
    }
}
