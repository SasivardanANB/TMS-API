using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMS.DomainObjects.Request
{
   public class ForgotPasswordRequest
    {
        [MaxLength(50)]
        public string Email { get; set; }
        [MaxLength(30)]
        public string UserName { get; set; }
    }
}
