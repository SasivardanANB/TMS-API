namespace DMS.DomainObjects.Objects
{
    public class Partner
    {
        public int ID { get; set; }
        public string PartnerNo { get; set; }
        public string PartnerName { get; set; }
        public int PartnerTypeID { get; set; }
        public bool IsActive { get; set; }
    }
}
