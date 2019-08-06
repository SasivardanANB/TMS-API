using System.ComponentModel.DataAnnotations;

namespace TMS.DomainObjects.Objects
{
    public class Region
    {
        public int ID { get; set; }
        [MaxLength(4)]
        public string BusinessAreaCode { get; set; }
        [MaxLength(100)]
        public string BusinessAreaDescription { get; set; }
        public int CompanyCodeID { get; set; }
        [MaxLength(200)]
        public string Address { get; set; }
        public int PostalCodeID { get; set; }
    }
}
