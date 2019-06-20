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
        [Required]
        public string Email { set; get; }
        [Required]
        public string URLLink { get; set; }
    }
}
