using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMS.DataGateway.DataModels
{
    public class DMSDBContext : DbContext
    {
        public DMSDBContext() : base("DMSDBConn") { }
        public virtual DbSet<BusinessArea> BusinessAreas { get; set; }
        public virtual DbSet<City> Cities { get; set; }
        public virtual DbSet<ImageGuId> ImageGuids { get; set; }
        public virtual DbSet<Partner> Partners { get; set; }
        public virtual DbSet<PartnerPartnerType> PartnerPartnerTypes { get; set; }
        public virtual DbSet<PartnerType> PartnerTypes { get; set; }
        public virtual DbSet<PostalCode> PostalCodes { get; set; }
        public virtual DbSet<Province> Provinces { get; set; }
        public virtual DbSet<ShipmentListDetails> ShipmentListDetails { get; set; }
        public virtual DbSet<StopPointImages> StopPointImages { get; set; }
        public virtual DbSet<SubDistrict> SubDistricts { get; set; }
        public virtual DbSet<TokenManager> TokenManagers { get; set; }
        public virtual DbSet<TripDetail> TripDetails { get; set; }
        public virtual DbSet<TripHeader> TripHeaders { get; set; }
        public virtual DbSet<TripStatus> TripStatuses { get; set; }
        public virtual DbSet<TripStatusHistory> TripStatusHistories { get; set; }
        public virtual DbSet<Driver> Drivers { get; set; }
        public virtual DbSet<ImageType> ImageTypes { get; set; }
        public virtual DbSet<DeviceToken> DeviceTokens { get; set; }
    }
}
