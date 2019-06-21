using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMS.DataGateway.DataModels
{
    public class OMSDBContext : DbContext
    {
        public OMSDBContext() : base("OMSDBConn") { }
        public virtual DbSet<Activity> Activities { get; set; }
        public virtual DbSet<Application> Applications { get; set; }
        public virtual DbSet<BusinessArea> BusinessAreas { get; set; }
        public virtual DbSet<City> Cities { get; set; }
        public virtual DbSet<CompanyCode> CompanyCodes { get; set; }
        public virtual DbSet<Menu> Menus { get; set; }
        public virtual DbSet<PostalCode> PostalCodes { get; set; }
        public virtual DbSet<Province> Provinces { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<RoleMenuActivity> RoleMenuActivity { get; set; }
        public virtual DbSet<RoleMenu> RoleMenus { get; set; }
        public virtual DbSet<SubDistrict> SubDistricts { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UserApplication> UserApplications { get; set; }
        public virtual DbSet<TokensManager> TokensManagers { get; set; }
        public virtual DbSet<MenuActivity> MenuActivities { get; set; }
        public virtual DbSet<OrderHeader> OrderHeaders { get; set; }
        public virtual DbSet<OrderDetail> OrderDetails { get; set; }
        public virtual DbSet<Partner> Partners { get; set; }
        public virtual DbSet<OrderPartnerDetail> OrderPartnerDetails { get; set; }
        public virtual DbSet<PartnerType> PartnerTypes { get; set; }
        public virtual DbSet<UserRoles> UserRoles { get; set; }
        public virtual DbSet<PackingSheet> PackingSheets { get; set; }
        public virtual DbSet<ShipmentSAP> ShipmentSAPs { get; set; }
        public virtual DbSet<OrderStatus> OrderStatuses { get; set; }
        public virtual DbSet<PartnerPartnerType> PartnerPartnerTypes { get; set; }
        public virtual DbSet<ImageGuid> ImageGuids { get; set; }
    }
}
