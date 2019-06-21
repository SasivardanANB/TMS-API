using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMS.DomainObjects.Objects
{
    public class Vehicle
    {
        public int ID { get; set; }
        [Required(ErrorMessageResourceType = typeof(Resource.ResourceData), ErrorMessageResourceName = "InvalidPlateNumber"), MaxLength(12, ErrorMessageResourceType = typeof(Resource.ResourceData), ErrorMessageResourceName = "InvalidPlateNumberLength"), MinLength(3, ErrorMessageResourceType = typeof(Resource.ResourceData), ErrorMessageResourceName = "InvalidPlateNumberLength")]
        public string PlateNumber { get; set; }
        [Required(ErrorMessageResourceType = typeof(Resource.ResourceData), ErrorMessageResourceName = "InvalidVehicleTypeID")]
        [Range(minimum: 1, maximum: int.MaxValue, ErrorMessageResourceType = typeof(Resource.ResourceData), ErrorMessageResourceName = "InvalidVehicleTypeID")]
        public int VehicleTypeID { get; set; }
        public string VehicleTypeDescription { get; set; }
        //[Required(ErrorMessageResourceType = typeof(Resource.ResourceData), ErrorMessageResourceName = "InvalidVehicleTypeName")]
        public string VehicleTypeName { get; set; }
        [Required(ErrorMessageResourceType = typeof(Resource.ResourceData), ErrorMessageResourceName = "InvalidPoliceNo")]
        [MaxLength(12, ErrorMessageResourceType = typeof(Resource.ResourceData), ErrorMessageResourceName = "InvalidPoliceNoLength"), MinLength(3, ErrorMessageResourceType = typeof(Resource.ResourceData), ErrorMessageResourceName = "InvalidPoliceNoLength")]
        public string PoliceNo { get; set; }
        [Required(ErrorMessageResourceType = typeof(Resource.ResourceData), ErrorMessageResourceName = "InvalidMaxWeight")]
        [Range(minimum: 1, maximum: int.MaxValue, ErrorMessageResourceType = typeof(Resource.ResourceData), ErrorMessageResourceName = "InvalidMaxWeight")]
        public decimal MaxWeight { get; set; }
        [Required(ErrorMessageResourceType = typeof(Resource.ResourceData), ErrorMessageResourceName = "InvalidMaxDimension"),MaxLength(12,ErrorMessageResourceType =typeof(Resource.ResourceData),ErrorMessageResourceName = "InvalidMaxDimensionLength"), MinLength(3, ErrorMessageResourceType = typeof(Resource.ResourceData), ErrorMessageResourceName = "InvalidMaxDimensionLength")]
        [RegularExpression("^[0-9]+\\*[0-9]+\\*[0-9]+$",ErrorMessage ="Invalid Max Dimensi format. Please provide in 12*34*56 format.")]
        public string MaxDimension { get; set; }
        [Required(ErrorMessageResourceType = typeof(Resource.ResourceData), ErrorMessageResourceName = "InvalidKIRNo"), MaxLength(25, ErrorMessageResourceType = typeof(Resource.ResourceData), ErrorMessageResourceName = "InvalidKIRNoLength"),MinLength(3, ErrorMessageResourceType = typeof(Resource.ResourceData), ErrorMessageResourceName = "InvalidKIRNoLength")]
        public string KIRNo { get; set; }
        [Required(ErrorMessageResourceType = typeof(Resource.ResourceData), ErrorMessageResourceName = "InvalidExpireDate")]
        public DateTime? KIRExpiryDate { get; set; }
        [Required(ErrorMessageResourceType = typeof(Resource.ResourceData), ErrorMessageResourceName = "InvalidPoolID")]
        [Range(minimum: 1, maximum: int.MaxValue, ErrorMessageResourceType = typeof(Resource.ResourceData), ErrorMessageResourceName = "InvalidPoolID")]
        public int PoolID { get; set; }
        [MaxLength(10)]
        public string PoolName { get; set; }
        public bool IsDedicated { get; set; }
        [Required(ErrorMessageResourceType = typeof(Resource.ResourceData), ErrorMessageResourceName = "InvalidShipperID")]
        [Range(minimum: 1, maximum: int.MaxValue, ErrorMessageResourceType = typeof(Resource.ResourceData), ErrorMessageResourceName = "InvalidShipperID")]
        public int ShipperID { get; set; }
        public string ShipperName { get; set; }
        public bool IsDelete { get; set; }
    }
}
