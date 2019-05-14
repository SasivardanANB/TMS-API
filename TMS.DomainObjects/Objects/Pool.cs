using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMS.DomainObjects.Objects
{
    public class Pool
    {
        public int ID { get; set; }
        public string PoolNo { get; set; }
        [Required(ErrorMessageResourceType = typeof(Resource.ResourceData), ErrorMessageResourceName = "InvalidPoolCode")]
        public string PoolName { get; set; }
        [Required(ErrorMessageResourceType = typeof(Resource.ResourceData), ErrorMessageResourceName = "InvalidPoolName")]
        public string PoolDescription { get; set; }
        [Required(ErrorMessageResourceType = typeof(Resource.ResourceData), ErrorMessageResourceName = "InvalidPhone")]
        public string Address { get; set; }
        [Required(ErrorMessageResourceType = typeof(Resource.ResourceData), ErrorMessageResourceName = "InvalidPhone")]
        public string ContactNumber { get; set; }
        [Required(ErrorMessageResourceType = typeof(Resource.ResourceData), ErrorMessageResourceName = "InvalidCityID")]
        [Range(minimum: 1, maximum: int.MaxValue, ErrorMessageResourceType = typeof(Resource.ResourceData), ErrorMessageResourceName = "InvalidCityID")]
        public int CityID { get; set; }
        public string CityName { get; set; }
        public int PhotoId { get; set; }
        [Required(ErrorMessageResourceType = typeof(Resource.ResourceData), ErrorMessageResourceName = "InvalidImage")]
        public string PhotoGuId { get; set; }
        public bool IsDelete { get; set; }
    }
}
