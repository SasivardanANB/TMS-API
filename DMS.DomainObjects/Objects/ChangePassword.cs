using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMS.DomainObjects.Objects
{
  public  class ChangePassword
    {
        public int Id { set; get; }
        public string Email { set; get; }
        public string OldPassword { set; get; }
        public string NewPassword { set; get; }
        public string ConformPassword { set; get; }
    }
}
