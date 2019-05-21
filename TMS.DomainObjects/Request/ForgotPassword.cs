using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMS.DomainObjects.Request
{
   public  class ForgotPassword
    {
        public int Id { set; get; }
        public string NewPassword { set; get; }
    }
}
