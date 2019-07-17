using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMS.DataGateway.DataModels
{
    public class TMSDBContext : DbContext
    {
        public TMSDBContext() : base("TMSDBConn") { }
        public virtual DbSet<OrderType> OrderTypes { get; set; }
        public virtual DbSet<Province> Provinces { get; set; }
        public virtual DbSet<City> Cities { get; set; }
        public virtual DbSet<SubDistrict> SubDistricts { get; set; }
        public virtual DbSet<PostalCode> PostalCodes { get; set; }
        public virtual DbSet<Menu> Menus { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<Activity> Activities { get; set; }
        public virtual DbSet<CompanyCode> CompanyCodes { get; set; }
        public virtual DbSet<BusinessArea> BusinessAreas { get; set; }
        public virtual DbSet<Partner> Partners { get; set; }
        public virtual DbSet<PartnerPIC> PartnerPICs { get; set; }
        public virtual DbSet<PartnerType> PartnerTypes { get; set; }
        public virtual DbSet<PartnerAddress> PartnerAddresses { get; set; }
        public virtual DbSet<OrderHeader> OrderHeaders { get; set; }
        public virtual DbSet<OrderDetail> OrderDetails { get; set; }
        public virtual DbSet<OrderPartnerDetail> OrderPartnerDetails { get; set; }
        public virtual DbSet<VehicleType> VehicleTypes { get; set; }
        public virtual DbSet<OrderStatusHistory> OrderStatusHistories { get; set; }
        public virtual DbSet<Driver> Drivers { get; set; }
        public virtual DbSet<Application> Applications { get; set; }
        public virtual DbSet<MenuActivity> MenuActivities { get; set; }
        public virtual DbSet<Token> Tokens { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<RoleMenu> RoleMenus { get; set; }
        public virtual DbSet<RoleMenuActivity> RoleMenuActivity { get; set; }
        public virtual DbSet<UserApplication> UserApplications { get; set; }
        public virtual DbSet<Vehicle> Vehicles { get; set; }
        public virtual DbSet<Pool> Pools { get; set; }
        public virtual DbSet<PIC> Pics  { get; set; }
        public virtual DbSet<UserRoles> UserRoles { get; set; }
        public virtual DbSet<ImageGuid> ImageGuids { get; set; }
        public virtual DbSet<OrderStatus> OrderStatuses { get; set; }
        public virtual DbSet<PackingSheet> PackingSheets { get; set; }
        public virtual DbSet<ShipmentSAP> ShipmentSAPs { get; set; }
        public virtual DbSet<FleetType> FleetTypes { get; set; }
        public virtual DbSet<G2G> G2Gs { get; set; }
        public virtual DbSet<GateType> GateTypes { get; set; }
        public virtual DbSet<GateInGateOut> GateInGateOuts { get; set; }
        public virtual DbSet<PartnerPartnerType> PartnerPartnerTypes { get; set; }
        public virtual DbSet<Harga> Hargas { get; set; }
    }
}
