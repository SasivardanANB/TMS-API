namespace OMS.DataGateway.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class OMS_v10_AllInitial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "OMS.Activity",
                c => new
                {
                    ID = c.Int(nullable: false, identity: true),
                    ActivityCode = c.String(nullable: false, maxLength: 50),
                    ActivityDescription = c.String(maxLength: 225),
                    CreatedBy = c.String(defaultValue: "SYSTEM"),
                    CreatedTime = c.DateTime(nullable: false, defaultValueSql: "GETDATE()"),
                    LastModifiedBy = c.String(),
                    LastModifiedTime = c.DateTime(),
                })
                .PrimaryKey(t => t.ID)
                .Index(t => t.ActivityCode, unique: true, name: "Activity_ActivityCode");

            CreateTable(
                "OMS.Application",
                c => new
                {
                    ID = c.Int(nullable: false, identity: true),
                    ApplicationCode = c.String(nullable: false, maxLength: 50),
                    ApplicationName = c.String(),
                    CreatedBy = c.String(defaultValue: "SYSTEM"),
                    CreatedTime = c.DateTime(nullable: false, defaultValueSql: "GETDATE()"),
                    LastModifiedBy = c.String(),
                    LastModifiedTime = c.DateTime(),
                })
                .PrimaryKey(t => t.ID)
                .Index(t => t.ApplicationCode, unique: true, name: "Application_ApplicationCode");

            CreateTable(
                "OMS.BusinessArea",
                c => new
                {
                    ID = c.Int(nullable: false, identity: true),
                    BusinessAreaCode = c.String(nullable: false, maxLength: 5),
                    BusinessAreaDescription = c.String(maxLength: 100),
                    CompanyCodeID = c.Int(),
                    Address = c.String(maxLength: 200),
                    PostalCodeID = c.Int(),
                    CreatedBy = c.String(defaultValue: "SYSTEM"),
                    CreatedTime = c.DateTime(nullable: false, defaultValueSql: "GETDATE()"),
                    LastModifiedBy = c.String(),
                    LastModifiedTime = c.DateTime(),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("OMS.CompanyCode", t => t.CompanyCodeID)
                .ForeignKey("OMS.PostalCode", t => t.PostalCodeID)
                .Index(t => t.BusinessAreaCode, unique: true, name: "BusinessArea_BusinessAreaCode")
                .Index(t => t.CompanyCodeID, unique: true, name: "BusinessArea_CompanyCodeID")
                .Index(t => t.PostalCodeID);

            CreateTable(
                "OMS.CompanyCode",
                c => new
                {
                    ID = c.Int(nullable: false, identity: true),
                    CompanyCodeCode = c.String(nullable: false, maxLength: 4),
                    CompanyCodeDescription = c.String(maxLength: 200),
                    CreatedBy = c.String(defaultValue: "SYSTEM"),
                    CreatedTime = c.DateTime(nullable: false, defaultValueSql: "GETDATE()"),
                    LastModifiedBy = c.String(),
                    LastModifiedTime = c.DateTime(),
                })
                .PrimaryKey(t => t.ID)
                .Index(t => t.CompanyCodeCode, unique: true, name: "CompanyCode_CompanyCodeCode");

            CreateTable(
                "OMS.PostalCode",
                c => new
                {
                    ID = c.Int(nullable: false, identity: true),
                    PostalCodeNo = c.String(nullable: false, maxLength: 6),
                    SubDistrictID = c.Int(nullable: false),
                    CreatedBy = c.String(defaultValue: "SYSTEM"),
                    CreatedTime = c.DateTime(nullable: false, defaultValueSql: "GETDATE()"),
                    LastModifiedBy = c.String(),
                    LastModifiedTime = c.DateTime(),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("OMS.SubDistrict", t => t.SubDistrictID, cascadeDelete: true)
                .Index(t => t.PostalCodeNo, unique: true, name: "PostalCode_PostalCodeNo")
                .Index(t => t.SubDistrictID);

            CreateTable(
                "OMS.SubDistrict",
                c => new
                {
                    ID = c.Int(nullable: false, identity: true),
                    SubdistrictCode = c.String(nullable: false, maxLength: 4),
                    SubdistrictName = c.String(maxLength: 50),
                    CityID = c.Int(nullable: false),
                    CreatedBy = c.String(defaultValue: "SYSTEM"),
                    CreatedTime = c.DateTime(nullable: false, defaultValueSql: "GETDATE()"),
                    LastModifiedBy = c.String(),
                    LastModifiedTime = c.DateTime(),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("OMS.City", t => t.CityID, cascadeDelete: true)
                .Index(t => t.SubdistrictCode, unique: true, name: "SubDistrict_SubdistrictCode")
                .Index(t => t.CityID);

            CreateTable(
                "OMS.City",
                c => new
                {
                    ID = c.Int(nullable: false, identity: true),
                    CityCode = c.String(nullable: false, maxLength: 4),
                    CityDescription = c.String(maxLength: 50),
                    ProvinceID = c.Int(nullable: false),
                    CreatedBy = c.String(defaultValue: "SYSTEM"),
                    CreatedTime = c.DateTime(nullable: false, defaultValueSql: "GETDATE()"),
                    LastModifiedBy = c.String(),
                    LastModifiedTime = c.DateTime(),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("OMS.Province", t => t.ProvinceID, cascadeDelete: true)
                .Index(t => t.CityCode, unique: true, name: "City_CityCode")
                .Index(t => t.ProvinceID);

            CreateTable(
                "OMS.Province",
                c => new
                {
                    ID = c.Int(nullable: false, identity: true),
                    ProvinceCode = c.String(nullable: false, maxLength: 4),
                    ProvinceDescription = c.String(maxLength: 50),
                    CreatedBy = c.String(defaultValue: "SYSTEM"),
                    CreatedTime = c.DateTime(nullable: false, defaultValueSql: "GETDATE()"),
                    LastModifiedBy = c.String(),
                    LastModifiedTime = c.DateTime(),
                })
                .PrimaryKey(t => t.ID)
                .Index(t => t.ProvinceCode, unique: true, name: "Province_ProvinceCode");

            CreateTable(
                "OMS.User",
                c => new
                {
                    ID = c.Int(nullable: false, identity: true),
                    UserName = c.String(nullable: false, maxLength: 50),
                    Password = c.String(),
                    FirstName = c.String(),
                    LastName = c.String(),
                    IsActive = c.Boolean(nullable: false),
                    IsDelete = c.Boolean(nullable: false),
                    CreatedBy = c.String(defaultValue: "SYSTEM"),
                    CreatedTime = c.DateTime(nullable: false, defaultValueSql: "GETDATE()"),
                    LastModifiedBy = c.String(),
                    LastModifiedTime = c.DateTime(),
                    BusinessArea_ID = c.Int(),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("OMS.BusinessArea", t => t.BusinessArea_ID)
                .Index(t => t.UserName, unique: true, name: "User_UserName")
                .Index(t => t.BusinessArea_ID);

            CreateTable(
                "OMS.FieldLabelMenu",
                c => new
                {
                    ID = c.Int(nullable: false, identity: true),
                    FieldLabel = c.String(maxLength: 30),
                    IsMandatory = c.Int(nullable: false),
                    IsDisplayed = c.Int(nullable: false),
                    IsReadOnly = c.Int(nullable: false),
                    DefaultValue = c.String(),
                    IsRunningNumber = c.Int(nullable: false),
                    IsAutoGeneratedv = c.Int(nullable: false),
                    FieldType = c.String(),
                    TableSave = c.String(),
                    FieldSave = c.String(),
                    IncludeObject = c.String(),
                    ExcludeObject = c.String(),
                    LogicCode = c.String(),
                    CreatedBy = c.String(defaultValue: "SYSTEM"),
                    CreatedTime = c.DateTime(nullable: false, defaultValueSql: "GETDATE()"),
                    LastModifiedBy = c.String(),
                    LastModifiedTime = c.DateTime(),
                    Menu_ID = c.Int(),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("OMS.Menu", t => t.Menu_ID)
                .Index(t => t.Menu_ID);

            CreateTable(
                "OMS.Menu",
                c => new
                {
                    ID = c.Int(nullable: false, identity: true),
                    MenuCode = c.String(nullable: false, maxLength: 5),
                    MenuDescription = c.String(maxLength: 50),
                    MenuURL = c.String(maxLength: 100),
                    CreatedBy = c.String(defaultValue: "SYSTEM"),
                    CreatedTime = c.DateTime(nullable: false, defaultValueSql: "GETDATE()"),
                    LastModifiedBy = c.String(),
                    LastModifiedTime = c.DateTime(),
                })
                .PrimaryKey(t => t.ID)
                .Index(t => t.MenuCode, unique: true, name: "Menu_MenuCode");

            CreateTable(
                "OMS.MenuActivity",
                c => new
                {
                    ID = c.Int(nullable: false, identity: true),
                    MenuID = c.Int(nullable: false),
                    ActivityID = c.Int(nullable: false),
                    CreatedBy = c.String(defaultValue: "SYSTEM"),
                    CreatedTime = c.DateTime(nullable: false, defaultValueSql: "GETDATE()"),
                    LastModifiedBy = c.String(),
                    LastModifiedTime = c.DateTime(),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("OMS.Activity", t => t.ActivityID, cascadeDelete: true)
                .ForeignKey("OMS.Menu", t => t.MenuID, cascadeDelete: true)
                .Index(t => t.MenuID)
                .Index(t => t.ActivityID);

            CreateTable(
                "OMS.OrderDetail",
                c => new
                {
                    ID = c.Int(nullable: false, identity: true),
                    OrderHeaderID = c.Int(nullable: false),
                    SequenceNo = c.Int(nullable: false),
                    Sender = c.String(),
                    Receiver = c.String(),
                    Dimension = c.String(),
                    TotalPallet = c.Int(nullable: false),
                    Instruction = c.String(),
                    ShippingListNo = c.String(),
                    TotalCollie = c.Int(nullable: false),
                    CreatedBy = c.String(defaultValue: "SYSTEM"),
                    CreatedTime = c.DateTime(nullable: false, defaultValueSql: "GETDATE()"),
                    LastModifiedBy = c.String(),
                    LastModifiedTime = c.DateTime(),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("OMS.OrderHeader", t => t.OrderHeaderID, cascadeDelete: true)
                .Index(t => t.OrderHeaderID);

            CreateTable(
                "OMS.OrderHeader",
                c => new
                {
                    ID = c.Int(nullable: false, identity: true),
                    BusinessAreaId = c.Int(nullable: false),
                    OrderNo = c.String(nullable: false, maxLength: 20),
                    LegecyOrderNo = c.String(),
                    OrderDate = c.DateTime(nullable: false),
                    OrderType = c.Int(nullable: false),
                    FleetType = c.Int(nullable: false),
                    VehicleShipment = c.String(),
                    DriverNo = c.String(),
                    DriverName = c.String(),
                    VehicleNo = c.String(),
                    OrderWeight = c.Decimal(nullable: false, precision: 18, scale: 2),
                    OrderWeightUM = c.String(),
                    EstimationShipmentDate = c.DateTime(nullable: false),
                    ActualShipmentDate = c.DateTime(nullable: false),
                    OrderShipmentStatus = c.Int(nullable: false),
                    IsActive = c.Boolean(nullable: false),
                    CreatedBy = c.String(defaultValue: "SYSTEM"),
                    CreatedTime = c.DateTime(nullable: false, defaultValueSql: "GETDATE()"),
                    LastModifiedBy = c.String(),
                    LastModifiedTime = c.DateTime(),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("OMS.BusinessArea", t => t.BusinessAreaId, cascadeDelete: true)
                .Index(t => t.BusinessAreaId)
                .Index(t => t.OrderNo, unique: true, name: "OrderHeader_OrderNo");

            CreateTable(
                "OMS.OrderPartnerDetail",
                c => new
                {
                    ID = c.Int(nullable: false, identity: true),
                    OrderDetailID = c.Int(nullable: false),
                    PartnerID = c.Int(nullable: false),
                    IsParent = c.Boolean(nullable: false),
                    IsOriginal = c.Boolean(nullable: false),
                    CreatedBy = c.String(defaultValue: "SYSTEM"),
                    CreatedTime = c.DateTime(nullable: false, defaultValueSql: "GETDATE()"),
                    LastModifiedBy = c.String(),
                    LastModifiedTime = c.DateTime(),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("OMS.OrderDetail", t => t.OrderDetailID, cascadeDelete: true)
                .ForeignKey("OMS.Partner", t => t.PartnerID, cascadeDelete: true)
                .Index(t => t.OrderDetailID)
                .Index(t => t.PartnerID);

            CreateTable(
                "OMS.Partner",
                c => new
                {
                    ID = c.Int(nullable: false, identity: true),
                    PartnerNo = c.String(maxLength: 10),
                    PartnerName = c.String(),
                    PartnerTypeID = c.Int(nullable: false),
                    IsActive = c.Boolean(nullable: false),
                    CreatedBy = c.String(defaultValue: "SYSTEM"),
                    CreatedTime = c.DateTime(nullable: false, defaultValueSql: "GETDATE()"),
                    LastModifiedBy = c.String(),
                    LastModifiedTime = c.DateTime(),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("OMS.PartnerType", t => t.PartnerTypeID, cascadeDelete: false)
                .Index(t => t.PartnerNo, unique: true, name: "Partner_PartnerNo")
                .Index(t => t.PartnerTypeID);

            CreateTable(
                "OMS.PartnerType",
                c => new
                {
                    ID = c.Int(nullable: false, identity: true),
                    PartnerTypeCode = c.String(nullable: false, maxLength: 10),
                    PartnerTypeDescription = c.String(),
                    CreatedBy = c.String(defaultValue: "SYSTEM"),
                    CreatedTime = c.DateTime(nullable: false, defaultValueSql: "GETDATE()"),
                    LastModifiedBy = c.String(),
                    LastModifiedTime = c.DateTime(),
                })
                .PrimaryKey(t => t.ID)
                .Index(t => t.PartnerTypeCode, unique: true, name: "PartnerType_PartnerTypeCode");

            CreateTable(
                "OMS.OrderStatus",
                c => new
                {
                    ID = c.Int(nullable: false, identity: true),
                    OrderStatusCode = c.String(),
                    OrderStatusValue = c.String(),
                    CreatedBy = c.String(defaultValue: "SYSTEM"),
                    CreatedTime = c.DateTime(nullable: false, defaultValueSql: "GETDATE()"),
                    LastModifiedBy = c.String(),
                    LastModifiedTime = c.DateTime(),
                })
                .PrimaryKey(t => t.ID);

            CreateTable(
                "OMS.PackingSheet",
                c => new
                {
                    ID = c.Int(nullable: false, identity: true),
                    OrderDetailID = c.Int(nullable: false),
                    PackingSheetNo = c.String(),
                    CreatedBy = c.String(defaultValue: "SYSTEM"),
                    CreatedTime = c.DateTime(nullable: false, defaultValueSql: "GETDATE()"),
                    LastModifiedBy = c.String(),
                    LastModifiedTime = c.DateTime(),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("OMS.OrderDetail", t => t.OrderDetailID, cascadeDelete: true)
                .Index(t => t.OrderDetailID);

            CreateTable(
                "OMS.RoleMenuActivity",
                c => new
                {
                    ID = c.Int(nullable: false, identity: true),
                    RoleMenuID = c.Int(nullable: false),
                    ActivityID = c.Int(nullable: false),
                    CreatedBy = c.String(defaultValue: "SYSTEM"),
                    CreatedTime = c.DateTime(nullable: false, defaultValueSql: "GETDATE()"),
                    LastModifiedBy = c.String(),
                    LastModifiedTime = c.DateTime(),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("OMS.Activity", t => t.ActivityID, cascadeDelete: true)
                .ForeignKey("OMS.RoleMenu", t => t.RoleMenuID, cascadeDelete: true)
                .Index(t => t.RoleMenuID)
                .Index(t => t.ActivityID);

            CreateTable(
                "OMS.RoleMenu",
                c => new
                {
                    ID = c.Int(nullable: false, identity: true),
                    RoleID = c.Int(nullable: false),
                    MenuID = c.Int(nullable: false),
                    CreatedBy = c.String(defaultValue: "SYSTEM"),
                    CreatedTime = c.DateTime(nullable: false, defaultValueSql: "GETDATE()"),
                    LastModifiedBy = c.String(),
                    LastModifiedTime = c.DateTime(),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("OMS.Menu", t => t.MenuID, cascadeDelete: true)
                .ForeignKey("OMS.Role", t => t.RoleID, cascadeDelete: true)
                .Index(t => t.RoleID)
                .Index(t => t.MenuID);

            CreateTable(
                "OMS.Role",
                c => new
                {
                    ID = c.Int(nullable: false, identity: true),
                    RoleCode = c.String(nullable: false, maxLength: 4),
                    RoleDescription = c.String(maxLength: 30),
                    ValidFrom = c.DateTime(nullable: false),
                    ValidTo = c.DateTime(nullable: false),
                    IsActive = c.Boolean(nullable: false),
                    IsDelete = c.Boolean(nullable: false),
                    CreatedBy = c.String(defaultValue: "SYSTEM"),
                    CreatedTime = c.DateTime(nullable: false, defaultValueSql: "GETDATE()"),
                    LastModifiedBy = c.String(),
                    LastModifiedTime = c.DateTime(),
                })
                .PrimaryKey(t => t.ID)
                .Index(t => t.RoleCode, unique: true, name: "Role_RoleCode");

            CreateTable(
                "OMS.ShipmentSAP",
                c => new
                {
                    ID = c.Int(nullable: false, identity: true),
                    OrderDetailID = c.Int(nullable: false),
                    ShipmentSAPNo = c.String(),
                    CreatedBy = c.String(defaultValue: "SYSTEM"),
                    CreatedTime = c.DateTime(nullable: false, defaultValueSql: "GETDATE()"),
                    LastModifiedBy = c.String(),
                    LastModifiedTime = c.DateTime(),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("OMS.OrderDetail", t => t.OrderDetailID, cascadeDelete: true)
                .Index(t => t.OrderDetailID);

            CreateTable(
                "OMS.TokensManager",
                c => new
                {
                    TokenID = c.Int(nullable: false, identity: true),
                    TokenKey = c.String(),
                    IssuedOn = c.DateTime(nullable: false),
                    ExpiresOn = c.DateTime(nullable: false),
                    CreatedOn = c.DateTime(nullable: false),
                    UserID = c.Int(nullable: false),
                    CreatedBy = c.String(defaultValue: "SYSTEM"),
                    CreatedTime = c.DateTime(nullable: false, defaultValueSql: "GETDATE()"),
                    LastModifiedBy = c.String(),
                    LastModifiedTime = c.DateTime(),
                })
                .PrimaryKey(t => t.TokenID)
                .ForeignKey("OMS.User", t => t.UserID, cascadeDelete: true)
                .Index(t => t.UserID);

            CreateTable(
                "OMS.UserApplication",
                c => new
                {
                    ID = c.Int(nullable: false, identity: true),
                    ApplicationID = c.Int(nullable: false),
                    UserID = c.Int(nullable: false),
                    CreatedBy = c.String(defaultValue: "SYSTEM"),
                    CreatedTime = c.DateTime(nullable: false, defaultValueSql: "GETDATE()"),
                    LastModifiedBy = c.String(),
                    LastModifiedTime = c.DateTime(),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("OMS.Application", t => t.ApplicationID, cascadeDelete: true)
                .ForeignKey("OMS.User", t => t.UserID, cascadeDelete: true)
                .Index(t => t.ApplicationID)
                .Index(t => t.UserID);

            CreateTable(
                "OMS.UserRoles",
                c => new
                {
                    ID = c.Int(nullable: false, identity: true),
                    UserID = c.Int(nullable: false),
                    RoleID = c.Int(nullable: false),
                    BusinessAreaID = c.Int(nullable: false),
                    IsDelete = c.Boolean(nullable: false),
                    CreatedBy = c.String(defaultValue: "SYSTEM"),
                    CreatedTime = c.DateTime(nullable: false, defaultValueSql: "GETDATE()"),
                    LastModifiedBy = c.String(),
                    LastModifiedTime = c.DateTime(),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("OMS.BusinessArea", t => t.BusinessAreaID, cascadeDelete: true)
                .ForeignKey("OMS.Role", t => t.RoleID, cascadeDelete: true)
                .ForeignKey("OMS.User", t => t.UserID, cascadeDelete: true)
                .Index(t => t.UserID)
                .Index(t => t.RoleID)
                .Index(t => t.BusinessAreaID);

        }

        public override void Down()
        {
            DropForeignKey("OMS.UserRoles", "UserID", "OMS.User");
            DropForeignKey("OMS.UserRoles", "RoleID", "OMS.Role");
            DropForeignKey("OMS.UserRoles", "BusinessAreaID", "OMS.BusinessArea");
            DropForeignKey("OMS.UserApplication", "UserID", "OMS.User");
            DropForeignKey("OMS.UserApplication", "ApplicationID", "OMS.Application");
            DropForeignKey("OMS.TokensManager", "UserID", "OMS.User");
            DropForeignKey("OMS.ShipmentSAP", "OrderDetailID", "OMS.OrderDetail");
            DropForeignKey("OMS.RoleMenuActivity", "RoleMenuID", "OMS.RoleMenu");
            DropForeignKey("OMS.RoleMenu", "RoleID", "OMS.Role");
            DropForeignKey("OMS.RoleMenu", "MenuID", "OMS.Menu");
            DropForeignKey("OMS.RoleMenuActivity", "ActivityID", "OMS.Activity");
            DropForeignKey("OMS.PackingSheet", "OrderDetailID", "OMS.OrderDetail");
            DropForeignKey("OMS.OrderPartnerDetail", "PartnerID", "OMS.Partner");
            DropForeignKey("OMS.Partner", "PartnerTypeID", "OMS.PartnerType");
            DropForeignKey("OMS.OrderPartnerDetail", "OrderDetailID", "OMS.OrderDetail");
            DropForeignKey("OMS.OrderDetail", "OrderHeaderID", "OMS.OrderHeader");
            DropForeignKey("OMS.OrderHeader", "BusinessAreaId", "OMS.BusinessArea");
            DropForeignKey("OMS.MenuActivity", "MenuID", "OMS.Menu");
            DropForeignKey("OMS.MenuActivity", "ActivityID", "OMS.Activity");
            DropForeignKey("OMS.FieldLabelMenu", "Menu_ID", "OMS.Menu");
            DropForeignKey("OMS.User", "BusinessArea_ID", "OMS.BusinessArea");
            DropForeignKey("OMS.BusinessArea", "PostalCodeID", "OMS.PostalCode");
            DropForeignKey("OMS.PostalCode", "SubDistrictID", "OMS.SubDistrict");
            DropForeignKey("OMS.SubDistrict", "CityID", "OMS.City");
            DropForeignKey("OMS.City", "ProvinceID", "OMS.Province");
            DropForeignKey("OMS.BusinessArea", "CompanyCodeID", "OMS.CompanyCode");
            DropIndex("OMS.UserRoles", new[] { "BusinessAreaID" });
            DropIndex("OMS.UserRoles", new[] { "RoleID" });
            DropIndex("OMS.UserRoles", new[] { "UserID" });
            DropIndex("OMS.UserApplication", new[] { "UserID" });
            DropIndex("OMS.UserApplication", new[] { "ApplicationID" });
            DropIndex("OMS.TokensManager", new[] { "UserID" });
            DropIndex("OMS.ShipmentSAP", new[] { "OrderDetailID" });
            DropIndex("OMS.Role", "Role_RoleCode");
            DropIndex("OMS.RoleMenu", new[] { "MenuID" });
            DropIndex("OMS.RoleMenu", new[] { "RoleID" });
            DropIndex("OMS.RoleMenuActivity", new[] { "ActivityID" });
            DropIndex("OMS.RoleMenuActivity", new[] { "RoleMenuID" });
            DropIndex("OMS.PackingSheet", new[] { "OrderDetailID" });
            DropIndex("OMS.PartnerType", "PartnerType_PartnerTypeCode");
            DropIndex("OMS.Partner", new[] { "PartnerTypeID" });
            DropIndex("OMS.Partner", "Partner_PartnerNo");
            DropIndex("OMS.OrderPartnerDetail", new[] { "PartnerID" });
            DropIndex("OMS.OrderPartnerDetail", new[] { "OrderDetailID" });
            DropIndex("OMS.OrderHeader", "OrderHeader_OrderNo");
            DropIndex("OMS.OrderHeader", new[] { "BusinessAreaId" });
            DropIndex("OMS.OrderDetail", new[] { "OrderHeaderID" });
            DropIndex("OMS.MenuActivity", new[] { "ActivityID" });
            DropIndex("OMS.MenuActivity", new[] { "MenuID" });
            DropIndex("OMS.Menu", "Menu_MenuCode");
            DropIndex("OMS.FieldLabelMenu", new[] { "Menu_ID" });
            DropIndex("OMS.User", new[] { "BusinessArea_ID" });
            DropIndex("OMS.User", "User_UserName");
            DropIndex("OMS.Province", "Province_ProvinceCode");
            DropIndex("OMS.City", new[] { "ProvinceID" });
            DropIndex("OMS.City", "City_CityCode");
            DropIndex("OMS.SubDistrict", new[] { "CityID" });
            DropIndex("OMS.SubDistrict", "SubDistrict_SubdistrictCode");
            DropIndex("OMS.PostalCode", new[] { "SubDistrictID" });
            DropIndex("OMS.PostalCode", "PostalCode_PostalCodeNo");
            DropIndex("OMS.CompanyCode", "CompanyCode_CompanyCodeCode");
            DropIndex("OMS.BusinessArea", new[] { "PostalCodeID" });
            DropIndex("OMS.BusinessArea", "BusinessArea_CompanyCodeID");
            DropIndex("OMS.BusinessArea", "BusinessArea_BusinessAreaCode");
            DropIndex("OMS.Application", "Application_ApplicationCode");
            DropIndex("OMS.Activity", "Activity_ActivityCode");
            DropTable("OMS.UserRoles");
            DropTable("OMS.UserApplication");
            DropTable("OMS.TokensManager");
            DropTable("OMS.ShipmentSAP");
            DropTable("OMS.Role");
            DropTable("OMS.RoleMenu");
            DropTable("OMS.RoleMenuActivity");
            DropTable("OMS.PackingSheet");
            DropTable("OMS.OrderStatus");
            DropTable("OMS.PartnerType");
            DropTable("OMS.Partner");
            DropTable("OMS.OrderPartnerDetail");
            DropTable("OMS.OrderHeader");
            DropTable("OMS.OrderDetail");
            DropTable("OMS.MenuActivity");
            DropTable("OMS.Menu");
            DropTable("OMS.FieldLabelMenu");
            DropTable("OMS.User");
            DropTable("OMS.Province");
            DropTable("OMS.City");
            DropTable("OMS.SubDistrict");
            DropTable("OMS.PostalCode");
            DropTable("OMS.CompanyCode");
            DropTable("OMS.BusinessArea");
            DropTable("OMS.Application");
            DropTable("OMS.Activity");
        }
    }
}
