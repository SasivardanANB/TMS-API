namespace TMS.DataGateway.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TMSDB_v10 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "TMS.Activity",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        ActivityCode = c.String(nullable: false, maxLength: 50),
                        ActivityDescription = c.String(maxLength: 225),
                        CreatedBy = c.String(maxLength: 100),
                        CreatedTime = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(maxLength: 100),
                        LastModifiedTime = c.DateTime(),
                    })
                .PrimaryKey(t => t.ID)
                .Index(t => t.ActivityCode, unique: true, name: "Activity_ActivityCode");
            
            CreateTable(
                "TMS.Application",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        ApplicationCode = c.String(nullable: false, maxLength: 50),
                        ApplicationName = c.String(maxLength: 100),
                        CreatedBy = c.String(maxLength: 100),
                        CreatedTime = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(maxLength: 100),
                        LastModifiedTime = c.DateTime(),
                    })
                .PrimaryKey(t => t.ID)
                .Index(t => t.ApplicationCode, unique: true, name: "Application_ApplicationCode");
            
            CreateTable(
                "TMS.BusinessArea",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        BusinessAreaCode = c.String(nullable: false, maxLength: 4),
                        BusinessAreaDescription = c.String(maxLength: 100),
                        CompanyCodeID = c.Int(),
                        Address = c.String(maxLength: 200),
                        PostalCodeID = c.Int(),
                        CreatedBy = c.String(maxLength: 100),
                        CreatedTime = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(maxLength: 100),
                        LastModifiedTime = c.DateTime(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("TMS.CompanyCode", t => t.CompanyCodeID)
                .ForeignKey("TMS.PostalCode", t => t.PostalCodeID)
                .Index(t => t.BusinessAreaCode, unique: true, name: "BusinessArea_BusinessAreaCode")
                .Index(t => t.CompanyCodeID)
                .Index(t => t.PostalCodeID);
            
            CreateTable(
                "TMS.CompanyCode",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        CompanyCodeCode = c.String(nullable: false, maxLength: 4),
                        CompanyCodeDescription = c.String(maxLength: 200),
                        CreatedBy = c.String(maxLength: 100),
                        CreatedTime = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(maxLength: 100),
                        LastModifiedTime = c.DateTime(),
                    })
                .PrimaryKey(t => t.ID)
                .Index(t => t.CompanyCodeCode, unique: true, name: "CompanyCode_CompanyCodeCode");
            
            CreateTable(
                "TMS.PostalCode",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        PostalCodeNo = c.String(nullable: false, maxLength: 6),
                        SubDistrictID = c.Int(nullable: false),
                        CreatedBy = c.String(maxLength: 100),
                        CreatedTime = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(maxLength: 100),
                        LastModifiedTime = c.DateTime(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("TMS.SubDistrict", t => t.SubDistrictID, cascadeDelete: true)
                .Index(t => t.PostalCodeNo, unique: true, name: "PostalCode_PostalCodeNo")
                .Index(t => t.SubDistrictID);
            
            CreateTable(
                "TMS.SubDistrict",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        SubdistrictCode = c.String(nullable: false, maxLength: 4),
                        SubdistrictName = c.String(maxLength: 50),
                        CityID = c.Int(nullable: false),
                        CreatedBy = c.String(maxLength: 100),
                        CreatedTime = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(maxLength: 100),
                        LastModifiedTime = c.DateTime(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("TMS.City", t => t.CityID, cascadeDelete: true)
                .Index(t => t.SubdistrictCode, unique: true, name: "SubDistrict_SubdistrictCode")
                .Index(t => t.CityID);
            
            CreateTable(
                "TMS.City",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        CityCode = c.String(nullable: false, maxLength: 4),
                        CityDescription = c.String(maxLength: 50),
                        ProvinceID = c.Int(nullable: false),
                        CreatedBy = c.String(maxLength: 100),
                        CreatedTime = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(maxLength: 100),
                        LastModifiedTime = c.DateTime(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("TMS.Province", t => t.ProvinceID, cascadeDelete: true)
                .Index(t => t.CityCode, unique: true, name: "City_CityCode")
                .Index(t => t.ProvinceID);
            
            CreateTable(
                "TMS.Province",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        ProvinceCode = c.String(nullable: false, maxLength: 4),
                        ProvinceDescription = c.String(maxLength: 50),
                        CreatedBy = c.String(maxLength: 100),
                        CreatedTime = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(maxLength: 100),
                        LastModifiedTime = c.DateTime(),
                    })
                .PrimaryKey(t => t.ID)
                .Index(t => t.ProvinceCode, unique: true, name: "Province_ProvinceCode");
            
            CreateTable(
                "TMS.Driver",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        DriverNo = c.String(maxLength: 12),
                        UserName = c.String(nullable: false, maxLength: 30),
                        FirstName = c.String(nullable: false, maxLength: 30),
                        LastName = c.String(nullable: false, maxLength: 30),
                        DriverAddress = c.String(maxLength: 200),
                        DriverPhone = c.String(nullable: false, maxLength: 15),
                        IsActive = c.Boolean(nullable: false),
                        Email = c.String(maxLength: 50),
                        Password = c.String(nullable: false, maxLength: 30),
                        IdentityNo = c.String(maxLength: 25),
                        DrivingLicenseNo = c.String(maxLength: 25),
                        DrivingLicenseExpiredDate = c.DateTime(),
                        IdentityImageId = c.Int(nullable: false),
                        DrivingLicenceImageId = c.Int(nullable: false),
                        DriverImageId = c.Int(nullable: false),
                        IsDelete = c.Boolean(nullable: false),
                        CreatedBy = c.String(maxLength: 100),
                        CreatedTime = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(maxLength: 100),
                        LastModifiedTime = c.DateTime(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("TMS.ImageGuid", t => t.IdentityImageId, cascadeDelete: true)
                .Index(t => t.DriverNo, unique: true, name: "Driver_DriverNo")
                .Index(t => t.IdentityNo, unique: true, name: "Driver_IdentityNo")
                .Index(t => t.DrivingLicenseNo, unique: true, name: "Driver_DrivingLicenseNo")
                .Index(t => t.IdentityImageId);
            
            CreateTable(
                "TMS.ImageGuid",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        ImageGuIdValue = c.String(maxLength: 1000),
                        IsActive = c.Boolean(nullable: false),
                        CreatedBy = c.String(maxLength: 100),
                        CreatedTime = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(maxLength: 100),
                        LastModifiedTime = c.DateTime(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "TMS.FleetType",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        FleetTypeDescription = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "TMS.G2G",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        BusinessAreaId = c.Int(nullable: false),
                        G2GName = c.String(maxLength: 50),
                        GateTypeId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("TMS.BusinessArea", t => t.BusinessAreaId, cascadeDelete: true)
                .ForeignKey("TMS.GateType", t => t.GateTypeId, cascadeDelete: true)
                .Index(t => t.BusinessAreaId)
                .Index(t => t.GateTypeId);
            
            CreateTable(
                "TMS.GateType",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        GateTypeDescription = c.String(maxLength: 10),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "TMS.GateInGateOut",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        OrderId = c.Int(nullable: false),
                        G2GId = c.Int(nullable: false),
                        GateTypeId = c.Int(nullable: false),
                        Info = c.String(maxLength: 120),
                        CreatedBy = c.String(maxLength: 100),
                        CreatedTime = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(maxLength: 100),
                        LastModifiedTime = c.DateTime(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("TMS.G2G", t => t.G2GId, cascadeDelete: true)
                .Index(t => t.G2GId);
            
            CreateTable(
                "TMS.MenuActivity",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        MenuID = c.Int(nullable: false),
                        ActivityID = c.Int(nullable: false),
                        CreatedBy = c.String(maxLength: 100),
                        CreatedTime = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(maxLength: 100),
                        LastModifiedTime = c.DateTime(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("TMS.Activity", t => t.ActivityID, cascadeDelete: true)
                .ForeignKey("TMS.Menu", t => t.MenuID, cascadeDelete: true)
                .Index(t => t.MenuID)
                .Index(t => t.ActivityID);
            
            CreateTable(
                "TMS.Menu",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        MenuCode = c.String(nullable: false, maxLength: 5),
                        MenuDescription = c.String(maxLength: 50),
                        MenuURL = c.String(maxLength: 100),
                        CreatedBy = c.String(maxLength: 100),
                        CreatedTime = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(maxLength: 100),
                        LastModifiedTime = c.DateTime(),
                    })
                .PrimaryKey(t => t.ID)
                .Index(t => t.MenuCode, unique: true, name: "Menu_MenuCode");
            
            CreateTable(
                "TMS.OrderDetail",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        OrderHeaderID = c.Int(nullable: false),
                        SequenceNo = c.Int(nullable: false),
                        Sender = c.String(maxLength: 50),
                        Receiver = c.String(maxLength: 50),
                        Dimension = c.String(maxLength: 11),
                        TotalPallet = c.Int(nullable: false),
                        Instruction = c.String(maxLength: 200),
                        ShippingListNo = c.String(maxLength: 20),
                        TotalCollie = c.Int(nullable: false),
                        EstimationShipmentDate = c.DateTime(nullable: false),
                        ActualShipmentDate = c.DateTime(nullable: false),
                        Katerangan = c.String(),
                        CreatedBy = c.String(maxLength: 100),
                        CreatedTime = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(maxLength: 100),
                        LastModifiedTime = c.DateTime(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("TMS.OrderHeader", t => t.OrderHeaderID, cascadeDelete: true)
                .Index(t => t.OrderHeaderID);
            
            CreateTable(
                "TMS.OrderHeader",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        BusinessAreaId = c.Int(nullable: false),
                        SOPONumber = c.String(maxLength: 10),
                        OrderNo = c.String(nullable: false, maxLength: 15),
                        LegecyOrderNo = c.String(maxLength: 15),
                        OrderDate = c.DateTime(nullable: false),
                        OrderType = c.Int(nullable: false),
                        FleetTypeID = c.Int(nullable: false),
                        VehicleShipment = c.String(maxLength: 50),
                        DriverNo = c.String(maxLength: 12),
                        DriverName = c.String(maxLength: 60),
                        VehicleNo = c.String(maxLength: 12),
                        OrderWeight = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OrderWeightUM = c.String(maxLength: 5),
                        OrderStatusID = c.Int(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        Harga = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ShipmentScheduleImageID = c.Int(),
                        CreatedBy = c.String(maxLength: 100),
                        CreatedTime = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(maxLength: 100),
                        LastModifiedTime = c.DateTime(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("TMS.BusinessArea", t => t.BusinessAreaId, cascadeDelete: true)
                .ForeignKey("TMS.FleetType", t => t.FleetTypeID, cascadeDelete: true)
                .ForeignKey("TMS.ImageGuid", t => t.ShipmentScheduleImageID)
                .Index(t => t.BusinessAreaId)
                .Index(t => t.OrderNo, unique: true, name: "OrderHeader_OrderNo")
                .Index(t => t.FleetTypeID)
                .Index(t => t.ShipmentScheduleImageID);
            
            CreateTable(
                "TMS.OrderPartnerDetail",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        OrderDetailID = c.Int(nullable: false),
                        PartnerID = c.Int(nullable: false),
                        IsParent = c.Boolean(nullable: false),
                        IsOriginal = c.Boolean(nullable: false),
                        PartnerTypeId = c.Int(nullable: false),
                        CreatedBy = c.String(maxLength: 100),
                        CreatedTime = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(maxLength: 100),
                        LastModifiedTime = c.DateTime(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("TMS.OrderDetail", t => t.OrderDetailID, cascadeDelete: true)
                .ForeignKey("TMS.Partner", t => t.PartnerID, cascadeDelete: true)
                .ForeignKey("TMS.PartnerType", t => t.PartnerTypeId, cascadeDelete: true)
                .Index(t => t.OrderDetailID)
                .Index(t => t.PartnerID)
                .Index(t => t.PartnerTypeId);
            
            CreateTable(
                "TMS.Partner",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        OrderPointTypeID = c.Int(),
                        OrderPointCode = c.String(maxLength: 10),
                        PartnerNo = c.String(maxLength: 10),
                        PartnerName = c.String(maxLength: 30),
                        PartnerAddress = c.String(maxLength: 200),
                        PostalCodeID = c.Int(),
                        PartnerInitial = c.String(maxLength: 30),
                        PartnerEmail = c.String(maxLength: 50),
                        PICID = c.Int(),
                        IsDeleted = c.Boolean(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        CreatedBy = c.String(maxLength: 100),
                        CreatedTime = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(maxLength: 100),
                        LastModifiedTime = c.DateTime(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("TMS.PIC", t => t.PICID)
                .ForeignKey("TMS.PostalCode", t => t.PostalCodeID)
                .Index(t => t.PostalCodeID)
                .Index(t => t.PICID);
            
            CreateTable(
                "TMS.PIC",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        PICName = c.String(maxLength: 60),
                        PICPhone = c.String(maxLength: 15),
                        PICEmail = c.String(maxLength: 50),
                        IsActive = c.Boolean(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        PICPassword = c.String(maxLength: 30),
                        PhotoId = c.Int(nullable: false),
                        CreatedBy = c.String(maxLength: 100),
                        CreatedTime = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(maxLength: 100),
                        LastModifiedTime = c.DateTime(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("TMS.ImageGuid", t => t.PhotoId, cascadeDelete: true)
                .Index(t => t.PhotoId);
            
            CreateTable(
                "TMS.PartnerType",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        PartnerTypeCode = c.String(nullable: false, maxLength: 4),
                        PartnerTypeDescription = c.String(maxLength: 50),
                        CreatedBy = c.String(maxLength: 100),
                        CreatedTime = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(maxLength: 100),
                        LastModifiedTime = c.DateTime(),
                    })
                .PrimaryKey(t => t.ID)
                .Index(t => t.PartnerTypeCode, unique: true, name: "PartnerType_PartnerTypeCode");
            
            CreateTable(
                "TMS.OrderStatus",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        OrderStatusCode = c.String(maxLength: 4),
                        OrderStatusValue = c.String(maxLength: 50),
                        CreatedBy = c.String(maxLength: 100),
                        CreatedTime = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(maxLength: 100),
                        LastModifiedTime = c.DateTime(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "TMS.OrderStatusHistory",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        OrderDetailID = c.Int(nullable: false),
                        OrderStatusID = c.Int(nullable: false),
                        StatusDate = c.DateTime(nullable: false),
                        Remarks = c.String(maxLength: 50),
                        IsLoad = c.Boolean(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("TMS.OrderDetail", t => t.OrderDetailID, cascadeDelete: true)
                .ForeignKey("TMS.OrderStatus", t => t.OrderStatusID, cascadeDelete: true)
                .Index(t => t.OrderDetailID)
                .Index(t => t.OrderStatusID);
            
            CreateTable(
                "TMS.OrderType",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        OrderTypeCode = c.String(nullable: false, maxLength: 4),
                        OrderTypeDescription = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => t.ID)
                .Index(t => t.OrderTypeCode, unique: true, name: "OrderType_OrderTypeCode");
            
            CreateTable(
                "TMS.PackingSheet",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        ShippingListNo = c.String(maxLength: 20),
                        PackingSheetNo = c.String(maxLength: 50),
                        CreatedBy = c.String(maxLength: 100),
                        CreatedTime = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(maxLength: 100),
                        LastModifiedTime = c.DateTime(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "TMS.PartnerAddress",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        PartnerID = c.Int(nullable: false),
                        Address = c.String(maxLength: 200),
                        Phone = c.String(maxLength: 15),
                        PostalCodeID = c.Int(nullable: false),
                        IsDefault = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .Index(t => t.PartnerID, unique: true, name: "PartnerAddress_PartnerID")
                .Index(t => t.Address, name: "PartnerAddress_Address");
            
            CreateTable(
                "TMS.PartnerPartnerType",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        PartnerId = c.Int(nullable: false),
                        PartnerTypeId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("TMS.Partner", t => t.PartnerId, cascadeDelete: true)
                .ForeignKey("TMS.PartnerType", t => t.PartnerTypeId, cascadeDelete: true)
                .Index(t => t.PartnerId)
                .Index(t => t.PartnerTypeId);
            
            CreateTable(
                "TMS.PartnerPIC",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        PartnerID = c.Int(nullable: false),
                        PICID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .Index(t => t.PartnerID, unique: true, name: "PartnerPIC_PartnerID")
                .Index(t => t.PICID, unique: true, name: "PartnerPIC_PICID");
            
            CreateTable(
                "TMS.Pool",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        PoolNo = c.String(maxLength: 15),
                        PoolName = c.String(maxLength: 10),
                        PoolDescription = c.String(maxLength: 50),
                        Address = c.String(maxLength: 200),
                        ContactNumber = c.String(maxLength: 15),
                        CityID = c.Int(nullable: false),
                        IsDelete = c.Boolean(nullable: false),
                        PhotoId = c.Int(nullable: false),
                        CreatedBy = c.String(maxLength: 100),
                        CreatedTime = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(maxLength: 100),
                        LastModifiedTime = c.DateTime(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("TMS.City", t => t.CityID, cascadeDelete: true)
                .ForeignKey("TMS.ImageGuid", t => t.PhotoId, cascadeDelete: true)
                .Index(t => t.PoolNo, unique: true, name: "Pool_PoolNo")
                .Index(t => t.PoolName, unique: true, name: "Pool_PoolName")
                .Index(t => t.CityID)
                .Index(t => t.PhotoId);
            
            CreateTable(
                "TMS.RoleMenuActivity",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        RoleMenuID = c.Int(nullable: false),
                        ActivityID = c.Int(nullable: false),
                        CreatedBy = c.String(maxLength: 100),
                        CreatedTime = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(maxLength: 100),
                        LastModifiedTime = c.DateTime(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("TMS.Activity", t => t.ActivityID, cascadeDelete: true)
                .ForeignKey("TMS.RoleMenu", t => t.RoleMenuID, cascadeDelete: true)
                .Index(t => t.RoleMenuID)
                .Index(t => t.ActivityID);
            
            CreateTable(
                "TMS.RoleMenu",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        RoleID = c.Int(nullable: false),
                        MenuID = c.Int(nullable: false),
                        CreatedBy = c.String(maxLength: 100),
                        CreatedTime = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(maxLength: 100),
                        LastModifiedTime = c.DateTime(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("TMS.Menu", t => t.MenuID, cascadeDelete: true)
                .ForeignKey("TMS.Role", t => t.RoleID, cascadeDelete: true)
                .Index(t => t.RoleID)
                .Index(t => t.MenuID);
            
            CreateTable(
                "TMS.Role",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        RoleCode = c.String(nullable: false, maxLength: 4),
                        RoleDescription = c.String(maxLength: 30),
                        IsActive = c.Boolean(nullable: false),
                        IsDelete = c.Boolean(nullable: false),
                        CreatedBy = c.String(maxLength: 100),
                        CreatedTime = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(maxLength: 100),
                        LastModifiedTime = c.DateTime(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "TMS.ShipmentSAP",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        OrderDetailID = c.Int(nullable: false),
                        ShipmentSAPNo = c.String(maxLength: 20),
                        CreatedBy = c.String(maxLength: 100),
                        CreatedTime = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(maxLength: 100),
                        LastModifiedTime = c.DateTime(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("TMS.OrderDetail", t => t.OrderDetailID, cascadeDelete: true)
                .Index(t => t.OrderDetailID);
            
            CreateTable(
                "TMS.Token",
                c => new
                    {
                        TokenID = c.Int(nullable: false, identity: true),
                        TokenKey = c.String(),
                        IssuedOn = c.DateTime(nullable: false),
                        ExpiresOn = c.DateTime(nullable: false),
                        CreatedOn = c.DateTime(nullable: false),
                        UserID = c.Int(nullable: false),
                        CreatedBy = c.String(maxLength: 100),
                        CreatedTime = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(maxLength: 100),
                        LastModifiedTime = c.DateTime(),
                    })
                .PrimaryKey(t => t.TokenID)
                .ForeignKey("TMS.User", t => t.UserID, cascadeDelete: true)
                .Index(t => t.UserID);
            
            CreateTable(
                "TMS.User",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        UserName = c.String(nullable: false, maxLength: 30),
                        Password = c.String(maxLength: 30),
                        FirstName = c.String(maxLength: 30),
                        LastName = c.String(maxLength: 30),
                        IsActive = c.Boolean(nullable: false),
                        IsDelete = c.Boolean(nullable: false),
                        CreatedBy = c.String(maxLength: 100),
                        CreatedTime = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(maxLength: 100),
                        LastModifiedTime = c.DateTime(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "TMS.UserApplication",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        ApplicationID = c.Int(nullable: false),
                        UserID = c.Int(nullable: false),
                        CreatedBy = c.String(maxLength: 100),
                        CreatedTime = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(maxLength: 100),
                        LastModifiedTime = c.DateTime(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("TMS.Application", t => t.ApplicationID, cascadeDelete: true)
                .ForeignKey("TMS.User", t => t.UserID, cascadeDelete: true)
                .Index(t => t.ApplicationID)
                .Index(t => t.UserID);
            
            CreateTable(
                "TMS.UserRoles",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        UserID = c.Int(nullable: false),
                        RoleID = c.Int(nullable: false),
                        BusinessAreaID = c.Int(nullable: false),
                        IsDelete = c.Boolean(nullable: false),
                        CreatedBy = c.String(maxLength: 100),
                        CreatedTime = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(maxLength: 100),
                        LastModifiedTime = c.DateTime(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("TMS.BusinessArea", t => t.BusinessAreaID, cascadeDelete: true)
                .ForeignKey("TMS.Role", t => t.RoleID, cascadeDelete: true)
                .ForeignKey("TMS.User", t => t.UserID, cascadeDelete: true)
                .Index(t => t.UserID)
                .Index(t => t.RoleID)
                .Index(t => t.BusinessAreaID);
            
            CreateTable(
                "TMS.Vehicle",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        PlateNumber = c.String(nullable: false, maxLength: 12),
                        VehicleTypeID = c.Int(nullable: false),
                        PoliceNo = c.String(maxLength: 12),
                        MaxWeight = c.Int(nullable: false),
                        MaxDimension = c.String(maxLength: 11),
                        KIRNo = c.String(maxLength: 25),
                        KIRExpiryDate = c.DateTime(),
                        PoolID = c.Int(nullable: false),
                        IsDedicated = c.Boolean(nullable: false),
                        ShipperID = c.Int(nullable: false),
                        IsDelete = c.Boolean(nullable: false),
                        CreatedBy = c.String(maxLength: 100),
                        CreatedTime = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(maxLength: 100),
                        LastModifiedTime = c.DateTime(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("TMS.Partner", t => t.ShipperID, cascadeDelete: true)
                .ForeignKey("TMS.Pool", t => t.PoolID, cascadeDelete: true)
                .ForeignKey("TMS.VehicleType", t => t.VehicleTypeID, cascadeDelete: true)
                .Index(t => t.VehicleTypeID)
                .Index(t => t.PoolID)
                .Index(t => t.ShipperID);
            
            CreateTable(
                "TMS.VehicleType",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        VehicleTypeCode = c.String(maxLength: 10),
                        VehicleTypeDescription = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => t.ID)
                .Index(t => t.VehicleTypeCode, unique: true, name: "VehicleType_VehicleTypeCode");
            
        }
        
        public override void Down()
        {
            DropForeignKey("TMS.Vehicle", "VehicleTypeID", "TMS.VehicleType");
            DropForeignKey("TMS.Vehicle", "PoolID", "TMS.Pool");
            DropForeignKey("TMS.Vehicle", "ShipperID", "TMS.Partner");
            DropForeignKey("TMS.UserRoles", "UserID", "TMS.User");
            DropForeignKey("TMS.UserRoles", "RoleID", "TMS.Role");
            DropForeignKey("TMS.UserRoles", "BusinessAreaID", "TMS.BusinessArea");
            DropForeignKey("TMS.UserApplication", "UserID", "TMS.User");
            DropForeignKey("TMS.UserApplication", "ApplicationID", "TMS.Application");
            DropForeignKey("TMS.Token", "UserID", "TMS.User");
            DropForeignKey("TMS.ShipmentSAP", "OrderDetailID", "TMS.OrderDetail");
            DropForeignKey("TMS.RoleMenuActivity", "RoleMenuID", "TMS.RoleMenu");
            DropForeignKey("TMS.RoleMenu", "RoleID", "TMS.Role");
            DropForeignKey("TMS.RoleMenu", "MenuID", "TMS.Menu");
            DropForeignKey("TMS.RoleMenuActivity", "ActivityID", "TMS.Activity");
            DropForeignKey("TMS.Pool", "PhotoId", "TMS.ImageGuid");
            DropForeignKey("TMS.Pool", "CityID", "TMS.City");
            DropForeignKey("TMS.PartnerPartnerType", "PartnerTypeId", "TMS.PartnerType");
            DropForeignKey("TMS.PartnerPartnerType", "PartnerId", "TMS.Partner");
            DropForeignKey("TMS.OrderStatusHistory", "OrderStatusID", "TMS.OrderStatus");
            DropForeignKey("TMS.OrderStatusHistory", "OrderDetailID", "TMS.OrderDetail");
            DropForeignKey("TMS.OrderPartnerDetail", "PartnerTypeId", "TMS.PartnerType");
            DropForeignKey("TMS.OrderPartnerDetail", "PartnerID", "TMS.Partner");
            DropForeignKey("TMS.Partner", "PostalCodeID", "TMS.PostalCode");
            DropForeignKey("TMS.Partner", "PICID", "TMS.PIC");
            DropForeignKey("TMS.PIC", "PhotoId", "TMS.ImageGuid");
            DropForeignKey("TMS.OrderPartnerDetail", "OrderDetailID", "TMS.OrderDetail");
            DropForeignKey("TMS.OrderDetail", "OrderHeaderID", "TMS.OrderHeader");
            DropForeignKey("TMS.OrderHeader", "ShipmentScheduleImageID", "TMS.ImageGuid");
            DropForeignKey("TMS.OrderHeader", "FleetTypeID", "TMS.FleetType");
            DropForeignKey("TMS.OrderHeader", "BusinessAreaId", "TMS.BusinessArea");
            DropForeignKey("TMS.MenuActivity", "MenuID", "TMS.Menu");
            DropForeignKey("TMS.MenuActivity", "ActivityID", "TMS.Activity");
            DropForeignKey("TMS.GateInGateOut", "G2GId", "TMS.G2G");
            DropForeignKey("TMS.G2G", "GateTypeId", "TMS.GateType");
            DropForeignKey("TMS.G2G", "BusinessAreaId", "TMS.BusinessArea");
            DropForeignKey("TMS.Driver", "IdentityImageId", "TMS.ImageGuid");
            DropForeignKey("TMS.BusinessArea", "PostalCodeID", "TMS.PostalCode");
            DropForeignKey("TMS.PostalCode", "SubDistrictID", "TMS.SubDistrict");
            DropForeignKey("TMS.SubDistrict", "CityID", "TMS.City");
            DropForeignKey("TMS.City", "ProvinceID", "TMS.Province");
            DropForeignKey("TMS.BusinessArea", "CompanyCodeID", "TMS.CompanyCode");
            DropIndex("TMS.VehicleType", "VehicleType_VehicleTypeCode");
            DropIndex("TMS.Vehicle", new[] { "ShipperID" });
            DropIndex("TMS.Vehicle", new[] { "PoolID" });
            DropIndex("TMS.Vehicle", new[] { "VehicleTypeID" });
            DropIndex("TMS.UserRoles", new[] { "BusinessAreaID" });
            DropIndex("TMS.UserRoles", new[] { "RoleID" });
            DropIndex("TMS.UserRoles", new[] { "UserID" });
            DropIndex("TMS.UserApplication", new[] { "UserID" });
            DropIndex("TMS.UserApplication", new[] { "ApplicationID" });
            DropIndex("TMS.Token", new[] { "UserID" });
            DropIndex("TMS.ShipmentSAP", new[] { "OrderDetailID" });
            DropIndex("TMS.RoleMenu", new[] { "MenuID" });
            DropIndex("TMS.RoleMenu", new[] { "RoleID" });
            DropIndex("TMS.RoleMenuActivity", new[] { "ActivityID" });
            DropIndex("TMS.RoleMenuActivity", new[] { "RoleMenuID" });
            DropIndex("TMS.Pool", new[] { "PhotoId" });
            DropIndex("TMS.Pool", new[] { "CityID" });
            DropIndex("TMS.Pool", "Pool_PoolName");
            DropIndex("TMS.Pool", "Pool_PoolNo");
            DropIndex("TMS.PartnerPIC", "PartnerPIC_PICID");
            DropIndex("TMS.PartnerPIC", "PartnerPIC_PartnerID");
            DropIndex("TMS.PartnerPartnerType", new[] { "PartnerTypeId" });
            DropIndex("TMS.PartnerPartnerType", new[] { "PartnerId" });
            DropIndex("TMS.PartnerAddress", "PartnerAddress_Address");
            DropIndex("TMS.PartnerAddress", "PartnerAddress_PartnerID");
            DropIndex("TMS.OrderType", "OrderType_OrderTypeCode");
            DropIndex("TMS.OrderStatusHistory", new[] { "OrderStatusID" });
            DropIndex("TMS.OrderStatusHistory", new[] { "OrderDetailID" });
            DropIndex("TMS.PartnerType", "PartnerType_PartnerTypeCode");
            DropIndex("TMS.PIC", new[] { "PhotoId" });
            DropIndex("TMS.Partner", new[] { "PICID" });
            DropIndex("TMS.Partner", new[] { "PostalCodeID" });
            DropIndex("TMS.OrderPartnerDetail", new[] { "PartnerTypeId" });
            DropIndex("TMS.OrderPartnerDetail", new[] { "PartnerID" });
            DropIndex("TMS.OrderPartnerDetail", new[] { "OrderDetailID" });
            DropIndex("TMS.OrderHeader", new[] { "ShipmentScheduleImageID" });
            DropIndex("TMS.OrderHeader", new[] { "FleetTypeID" });
            DropIndex("TMS.OrderHeader", "OrderHeader_OrderNo");
            DropIndex("TMS.OrderHeader", new[] { "BusinessAreaId" });
            DropIndex("TMS.OrderDetail", new[] { "OrderHeaderID" });
            DropIndex("TMS.Menu", "Menu_MenuCode");
            DropIndex("TMS.MenuActivity", new[] { "ActivityID" });
            DropIndex("TMS.MenuActivity", new[] { "MenuID" });
            DropIndex("TMS.GateInGateOut", new[] { "G2GId" });
            DropIndex("TMS.G2G", new[] { "GateTypeId" });
            DropIndex("TMS.G2G", new[] { "BusinessAreaId" });
            DropIndex("TMS.Driver", new[] { "IdentityImageId" });
            DropIndex("TMS.Driver", "Driver_DrivingLicenseNo");
            DropIndex("TMS.Driver", "Driver_IdentityNo");
            DropIndex("TMS.Driver", "Driver_DriverNo");
            DropIndex("TMS.Province", "Province_ProvinceCode");
            DropIndex("TMS.City", new[] { "ProvinceID" });
            DropIndex("TMS.City", "City_CityCode");
            DropIndex("TMS.SubDistrict", new[] { "CityID" });
            DropIndex("TMS.SubDistrict", "SubDistrict_SubdistrictCode");
            DropIndex("TMS.PostalCode", new[] { "SubDistrictID" });
            DropIndex("TMS.PostalCode", "PostalCode_PostalCodeNo");
            DropIndex("TMS.CompanyCode", "CompanyCode_CompanyCodeCode");
            DropIndex("TMS.BusinessArea", new[] { "PostalCodeID" });
            DropIndex("TMS.BusinessArea", new[] { "CompanyCodeID" });
            DropIndex("TMS.BusinessArea", "BusinessArea_BusinessAreaCode");
            DropIndex("TMS.Application", "Application_ApplicationCode");
            DropIndex("TMS.Activity", "Activity_ActivityCode");
            DropTable("TMS.VehicleType");
            DropTable("TMS.Vehicle");
            DropTable("TMS.UserRoles");
            DropTable("TMS.UserApplication");
            DropTable("TMS.User");
            DropTable("TMS.Token");
            DropTable("TMS.ShipmentSAP");
            DropTable("TMS.Role");
            DropTable("TMS.RoleMenu");
            DropTable("TMS.RoleMenuActivity");
            DropTable("TMS.Pool");
            DropTable("TMS.PartnerPIC");
            DropTable("TMS.PartnerPartnerType");
            DropTable("TMS.PartnerAddress");
            DropTable("TMS.PackingSheet");
            DropTable("TMS.OrderType");
            DropTable("TMS.OrderStatusHistory");
            DropTable("TMS.OrderStatus");
            DropTable("TMS.PartnerType");
            DropTable("TMS.PIC");
            DropTable("TMS.Partner");
            DropTable("TMS.OrderPartnerDetail");
            DropTable("TMS.OrderHeader");
            DropTable("TMS.OrderDetail");
            DropTable("TMS.Menu");
            DropTable("TMS.MenuActivity");
            DropTable("TMS.GateInGateOut");
            DropTable("TMS.GateType");
            DropTable("TMS.G2G");
            DropTable("TMS.FleetType");
            DropTable("TMS.ImageGuid");
            DropTable("TMS.Driver");
            DropTable("TMS.Province");
            DropTable("TMS.City");
            DropTable("TMS.SubDistrict");
            DropTable("TMS.PostalCode");
            DropTable("TMS.CompanyCode");
            DropTable("TMS.BusinessArea");
            DropTable("TMS.Application");
            DropTable("TMS.Activity");
        }
    }
}
