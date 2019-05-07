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
        public virtual DbSet<NumberingRange> NumberingRanges { get; set; }
        public virtual DbSet<CompanyCodeNumberingRange> CompanyCodeNumberingRanges { get; set; }
        public virtual DbSet<PartnerRole> PartnerRoles { get; set; }
        public virtual DbSet<OrderPointType> OrderPointTypes { get; set; }
        public virtual DbSet<Partner> Partners { get; set; }
        public virtual DbSet<PartnerPIC> PartnerPICs { get; set; }
        public virtual DbSet<PartnerType> PartnerTypes { get; set; }
        public virtual DbSet<PartnerAddress> PartnerAddresses { get; set; }
        public virtual DbSet<PartnerTypeFunction> PartnerTypeFunctions { get; set; }
        public virtual DbSet<OrderHeader> OrderHeaders { get; set; }
        public virtual DbSet<OrderDetail> OrderDetails { get; set; }
        public virtual DbSet<OrderPartnerDetail> OrderPartnerDetails { get; set; }
        public virtual DbSet<MappingOrderPartner> MappingOrderPartners { get; set; }
        public virtual DbSet<VehicleType> VehicleTypes { get; set; }
        public virtual DbSet<OrderStatusHistory> OrderStatusHistories { get; set; }
        public virtual DbSet<OrderTypeStatusWorkFlow> OrderTypeStatusWorkFlows { get; set; }
        public virtual DbSet<OrderHeaderHSOAdditionalData> OrderHeaderHSOAdditionalDatas { get; set; }
        public virtual DbSet<TripHeader> TripHeaders { get; set; }
        public virtual DbSet<TripDetail> TripDetails { get; set; }
        public virtual DbSet<TripPartnerDetail> TripPartnerDetails { get; set; }
        public virtual DbSet<TripStatus> TripStatuses { get; set; }
        public virtual DbSet<TripStatusHistory> TripStatusHistories { get; set; }
        public virtual DbSet<OrderTripStatusWorkFlow> OrderTripStatusWorkFlows { get; set; }
        public virtual DbSet<TripType> TripTypes { get; set; }
        public virtual DbSet<BillingTrip> BillingTrips { get; set; }
        public virtual DbSet<BillingTripDetail> BillingTripDetails { get; set; }
        public virtual DbSet<BillingTripPartnerDetail> BillingTripPartnerDetails { get; set; }
        public virtual DbSet<TermsOfPayment> TermsOfPayments { get; set; }
        public virtual DbSet<CancellationReason> CancellationReasons { get; set; }
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
    }
}
