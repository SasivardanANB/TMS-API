using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
namespace OMS.DomainObjects.Objects
{
    public class RoleMenu
    {
        public int ID { get; set; }
        //[Required(ErrorMessageResourceType = typeof(Resource.ResourceData), ErrorMessageResourceName = "InvalidMenuCode")]
        public string MenuCode { get; set; }
        public string MenuDescription { get; set; }
        public string MenuURL { get; set; }
        public List<RoleMenuActivity> RoleMenuActivities { get; set; }
    }
}
