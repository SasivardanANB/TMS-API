using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMS.DomainObjects.Request
{
  public  class ForgotPasswordRequest
    {
        [MaxLength(50)]
        public string Email { set; get; }
        public string URLLink { get; set; }
    }
}
