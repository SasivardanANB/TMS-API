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
        public virtual DbSet<City> Cities { get; set; }
        public virtual DbSet<Location> Locations { get; set; }
        public virtual DbSet<PostalCode> PostalCodes { get; set; }
        public virtual DbSet<Province> Provinces { get; set; }
        public virtual DbSet<ShipmentListDetails> ShipmentListDetails { get; set; }
        public virtual DbSet<StopPointImages> StopPointImages { get; set; }
        public virtual DbSet<StopPoints> StopPoints { get; set; }
        public virtual DbSet<SubDistrict> SubDistricts { get; set; }
        public virtual DbSet<TokenManager> TokenManagers { get; set; }
        public virtual DbSet<TripDetails> TripDetails { get; set; }
        public virtual DbSet<TripStatus> TripStatuses { get; set; }
        public virtual DbSet<TripStatusEventLog> TripStatusEventLogs { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<ImageGuid> ImageGuids { get; set; }
        public virtual DbSet<TripGuid> TripGuids { get; set; }
        public virtual DbSet<Pod> Pods { get; set; }
    }
}
