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
        [MaxLength(30)]
        public string OldPassword { set; get; }
        [Required]
        [MaxLength(30)]
        public string NewPassword { set; get; }
    }
}
