using System.Collections.Generic;

namespace TMS.DomainObjects.Objects
{
    public class RoleMenu
    {
        public int ID { get; set; }
        public string MenuCode { get; set; }
        public string MenuDescription { get; set; }
        public string MenuURL { get; set; }
        public List<RoleMenuActivity> RoleMenuActivities { get; set; }
    }
}
