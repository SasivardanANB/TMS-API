using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace DMS.DomainObjects.Request
{
  public  class ChangePasswordRequest
    {
        public int Id { set; get; }
        [Required]
        public string OldPassword { set; get; }
        [Required]
        public string NewPassword { set; get; }
        [Required]
        public string ConfirmNewPassword { set; get; }
    }
}
