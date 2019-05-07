namespace TMS.DataGateway.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
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
                    })
                .PrimaryKey(t => t.ID)
                .Index(t => t.ActivityCode, unique: true, name: "Activity_ActivityCode");
            
            CreateTable(
                "TMS.Application",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        ApplicationCode = c.String(nullable: false, maxLength: 50),
                        ApplicationName = c.String(),
                    })
                .PrimaryKey(t => t.ID)
                .Index(t => t.ApplicationCode, unique: true, name: "Application_ApplicationCode");
            
            CreateTable(
                "TMS.BillingTripDetail",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        BillingHeaderID = c.Int(nullable: false),
                        ItemNo = c.Int(nullable: false),
                        TripDetailID = c.Int(nullable: false),
                        ItemStatus = c.Int(nullable: false),
                        Berat = c.Decimal(nullable: false, precision: 18, scale: 2),
                        KM = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Price = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Discount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        VATAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.ID)
                .Index(t => t.BillingHeaderID, unique: true, name: "BillingTripDetail_BillingHeaderID")
                .Index(t => t.ItemNo, unique: true, name: "BillingTripDetail_ItemNo")
                .Index(t => t.TripDetailID, unique: true, name: "BillingTripDetail_TripDetailID");
            
            CreateTable(
                "TMS.BillingTripPartnerDetail",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        BillingDetailID = c.Int(nullable: false),
                        PartnerTypeID = c.Int(nullable: false),
                        PartnerID = c.Int(nullable: false),
                        ParentID = c.Int(nullable: false),
                        IsParent = c.Int(nullable: false),
                        IsOriginal = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .Index(t => t.BillingDetailID, unique: true, name: "BillingTripPartnerDetail_BillingDetailID")
                .Index(t => t.PartnerTypeID, unique: true, name: "BillingTripPartnerDetail_PartnerTypeID")
                .Index(t => t.PartnerID, unique: true, name: "BillingTripPartnerDetail_PartnerID");
            
            CreateTable(
                "TMS.BillingTrip",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        CompanyCodeID = c.Int(nullable: false),
                        BusinessAreaID = c.Int(nullable: false),
                        BillingNo = c.String(maxLength: 10),
                        BillingDate = c.DateTime(nullable: false),
                        BillingTypeID = c.Int(nullable: false),
                        BillingStatusID = c.Int(nullable: false),
                        TermsOfPaymentID = c.Int(nullable: false),
                        CancellationReasonID = c.Int(nullable: false),
                        CancellationReasonDescription = c.String(),
                        TotalNetValue = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalVATAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalDiscount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalIncVAT = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.ID)
                .Index(t => t.CompanyCodeID, unique: true, name: "BillingTrip_CompanyCodeID")
                .Index(t => t.BusinessAreaID, unique: true, name: "BillingTrip_BusinessAreaID")
                .Index(t => t.BillingNo, unique: true, name: "BillingTrip_BillingNo");
            
            CreateTable(
                "TMS.BusinessArea",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        BusinessAreaCode = c.String(nullable: false, maxLength: 5),
                        BusinessAreaDescription = c.String(maxLength: 100),
                        CompanyCodeID = c.Int(nullable: false),
                        Address = c.String(maxLength: 200),
                        PostalCodeID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .Index(t => t.BusinessAreaCode, unique: true, name: "BusinessArea_BusinessAreaCode")
                .Index(t => t.CompanyCodeID, unique: true, name: "BusinessArea_CompanyCodeID");
            
            CreateTable(
                "TMS.CancellationReason",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        CancellationReasonCode = c.String(maxLength: 10),
                        CancellationReasonDescription = c.String(),
                    })
                .PrimaryKey(t => t.ID)
                .Index(t => t.CancellationReasonCode, unique: true, name: "CancellationReason_CancellationReasonCode");
            
            CreateTable(
                "TMS.City",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        CityCode = c.String(nullable: false, maxLength: 4),
                        CityDescription = c.String(maxLength: 50),
                        ProvinceID = c.Int(nullable: false),
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
                    })
                .PrimaryKey(t => t.ID)
                .Index(t => t.ProvinceCode, unique: true, name: "Province_ProvinceCode");
            
            CreateTable(
                "TMS.CompanyCodeNumberingRange",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        CompanyCodeCode = c.String(nullable: false, maxLength: 4),
                        BusinessAreaCode = c.String(maxLength: 10),
                        IsNumberRange = c.Int(nullable: false),
                        TransactionTypeCode = c.String(maxLength: 10),
                        IsDisplayOnly = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .Index(t => t.CompanyCodeCode, unique: true, name: "CompanyCodeNumberingRange_CompanyCodeCode")
                .Index(t => t.BusinessAreaCode, unique: true, name: "CompanyCodeNumberingRange_BusinessAreaCode")
                .Index(t => t.TransactionTypeCode, unique: true, name: "CompanyCodeNumberingRange_TransactionTypeCode");
            
            CreateTable(
                "TMS.CompanyCode",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        CompanyCodeCode = c.String(nullable: false, maxLength: 4),
                        CompanyCodeDescription = c.String(maxLength: 200),
                    })
                .PrimaryKey(t => t.ID)
                .Index(t => t.CompanyCodeCode, unique: true, name: "CompanyCode_CompanyCodeCode");
            
            CreateTable(
                "TMS.Driver",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        DriverNo = c.String(maxLength: 12),
                        DriverName = c.String(maxLength: 30),
                        DriverAddress = c.String(maxLength: 255),
                        DriverPhone = c.String(maxLength: 15),
                        IsActive = c.Boolean(nullable: false),
                        Email = c.String(maxLength: 50),
                        Password = c.String(maxLength: 20),
                        IdentityNo = c.String(maxLength: 25),
                        DrivingLicenseNo = c.String(maxLength: 25),
                        DrivingLicenseExpiredDate = c.DateTime(nullable: false),
                        IdentityImage = c.String(maxLength: 200),
                        DrivingLicenceImage = c.String(maxLength: 200),
                        DriverImage = c.String(maxLength: 200),
                    })
                .PrimaryKey(t => t.ID)
                .Index(t => t.DriverNo, unique: true, name: "Driver_DriverNo")
                .Index(t => t.IdentityNo, unique: true, name: "Driver_IdentityNo")
                .Index(t => t.DrivingLicenseNo, unique: true, name: "Driver_DrivingLicenseNo");
            
            CreateTable(
                "TMS.MappingOrderPartner",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        OrderTypeID = c.Int(nullable: false),
                        PartnerTypeID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .Index(t => t.OrderTypeID, unique: true, name: "MappingOrderPartner_OrderTypeID")
                .Index(t => t.PartnerTypeID, unique: true, name: "MappingOrderPartner_PartnerTypeID");
            
            CreateTable(
                "TMS.MenuActivity",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        MenuID = c.Int(nullable: false),
                        ActivityID = c.Int(nullable: false),
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
                    })
                .PrimaryKey(t => t.ID)
                .Index(t => t.MenuCode, unique: true, name: "Menu_MenuCode");
            
            CreateTable(
                "TMS.NumberingRange",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        CompanyCodeCode = c.String(nullable: false, maxLength: 10),
                        BusinessAreaCode = c.String(maxLength: 10),
                        TransactionTypeCode = c.String(maxLength: 10),
                        Prefix = c.String(),
                        StartNumber = c.Int(nullable: false),
                        EndNumber = c.Int(nullable: false),
                        LastNumber = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .Index(t => t.CompanyCodeCode, unique: true, name: "NumberingRange_CompanyCodeCode")
                .Index(t => t.BusinessAreaCode, unique: true, name: "NumberingRange_BusinessAreaCode")
                .Index(t => t.TransactionTypeCode, unique: true, name: "NumberingRange_TransactionTypeCode");
            
            CreateTable(
                "TMS.OrderDetail",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        OrderHeaderID = c.Int(nullable: false),
                        ItemNo = c.Int(nullable: false),
                        Pengirim = c.String(),
                        Penerima = c.String(),
                        Instruksi = c.String(),
                        EstimatedArrivalTime = c.DateTime(nullable: false),
                        ActualArrivalTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .Index(t => t.OrderHeaderID, unique: true, name: "OrderDetail_OrderHeaderID")
                .Index(t => t.ItemNo, unique: true, name: "OrderDetail_ItemNo");
            
            CreateTable(
                "TMS.OrderHeaderHSOAdditionalData",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        OrderHeaderID = c.Int(nullable: false),
                        ShipmentIDAHM = c.String(),
                    })
                .PrimaryKey(t => t.ID)
                .Index(t => t.OrderHeaderID, unique: true, name: "OrderHeaderHSOAdditionalData_OrderHeaderID");
            
            CreateTable(
                "TMS.OrderHeader",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        CompanyCodeID = c.String(maxLength: 4),
                        BusinessAreaID = c.String(),
                        TipeOrder = c.String(maxLength: 10),
                        OrderNo = c.String(maxLength: 10),
                        OrderDate = c.DateTime(nullable: false),
                        EstimatedPickupTime = c.DateTime(nullable: false),
                        ActualPickupTime = c.DateTime(nullable: false),
                        EstimatedArrivalTime = c.DateTime(nullable: false),
                        ActualArrivalTime = c.DateTime(nullable: false),
                        TotalPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OrderStatusID = c.Int(nullable: false),
                        Sender = c.String(),
                        Receiver = c.String(),
                        Instruction = c.String(),
                        VehicleType = c.String(),
                        PoliceNo = c.String(),
                        TotalOfWeight = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .Index(t => t.TipeOrder, unique: true, name: "OrderHeader_TipeOrder")
                .Index(t => t.OrderNo, unique: true, name: "OrderHeader_OrderNo");
            
            CreateTable(
                "TMS.OrderPartnerDetail",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        OrderDetailID = c.Int(nullable: false),
                        PartnerTypeID = c.Int(nullable: false),
                        PartnerID = c.Int(nullable: false),
                        ParentID = c.Int(nullable: false),
                        IsParent = c.Int(nullable: false),
                        IsOriginal = c.Int(nullable: false),
                        CustomerName = c.String(),
                        CustomerPhone = c.String(),
                        CustomerAddress = c.String(maxLength: 200),
                        Longitude = c.String(),
                        Lattitude = c.String(),
                    })
                .PrimaryKey(t => t.ID)
                .Index(t => t.OrderDetailID, unique: true, name: "OrderPartnerDetail_OrderDetailID")
                .Index(t => t.PartnerTypeID, unique: true, name: "OrderPartnerDetail_PartnerTypeID")
                .Index(t => t.PartnerID, unique: true, name: "OrderPartnerDetail_PartnerID");
            
            CreateTable(
                "TMS.OrderPointType",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        PartnerRoleCode = c.String(nullable: false, maxLength: 10),
                        PartnerRoleDescription = c.String(),
                    })
                .PrimaryKey(t => t.ID)
                .Index(t => t.PartnerRoleCode, unique: true, name: "OrderPointType_PartnerRoleCode");
            
            CreateTable(
                "TMS.OrderStatusHistory",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        OrderHeaderID = c.Int(nullable: false),
                        OrderStatus = c.Int(nullable: false),
                        StepNo = c.Int(nullable: false),
                        IsOptional = c.Int(nullable: false),
                        CreatedTime = c.DateTime(nullable: false),
                        CreatedBy = c.DateTime(nullable: false),
                        LastModififiedTime = c.DateTime(nullable: false),
                        LastModifiedBy = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .Index(t => t.OrderHeaderID, unique: true, name: "OrderStatusHistory_OrderHeaderID")
                .Index(t => t.OrderStatus, unique: true, name: "OrderStatusHistory_OrderStatus")
                .Index(t => t.StepNo, unique: true, name: "OrderStatusHistory_StepNo");
            
            CreateTable(
                "TMS.OrderTripStatusWorkFlow",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        TripTypeID = c.Int(nullable: false),
                        TripStatusID = c.Int(nullable: false),
                        StepNo = c.Int(nullable: false),
                        IsOptional = c.Int(nullable: false),
                        IsTrackable = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .Index(t => t.TripTypeID, unique: true, name: "OrderTripStatusWorkFlow_TripTypeID")
                .Index(t => t.TripStatusID, unique: true, name: "OrderTripStatusWorkFlow_TripStatusID")
                .Index(t => t.StepNo, unique: true, name: "OrderTripStatusWorkFlow_StepNo");
            
            CreateTable(
                "TMS.OrderType",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        OrderTypeCode = c.String(nullable: false, maxLength: 4),
                        OrderTypeDescription = c.String(maxLength: 30),
                    })
                .PrimaryKey(t => t.ID)
                .Index(t => t.OrderTypeCode, unique: true, name: "OrderType_OrderTypeCode");
            
            CreateTable(
                "TMS.OrderTypeStatusWorkFlow",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        TipeOrderID = c.Int(nullable: false),
                        OrderStatus = c.Int(nullable: false),
                        StepNo = c.Int(nullable: false),
                        IsOptional = c.Int(nullable: false),
                        isTrackable = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .Index(t => t.TipeOrderID, unique: true, name: "OrderTypeStatusWorkFlow_TipeOrderID")
                .Index(t => t.OrderStatus, unique: true, name: "OrderTypeStatusWorkFlow_OrderStatus")
                .Index(t => t.StepNo, unique: true, name: "OrderTypeStatusWorkFlow_StepNo");
            
            CreateTable(
                "TMS.PartnerAddress",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        PartnerID = c.Int(nullable: false),
                        Address = c.String(maxLength: 200),
                        Phone = c.String(),
                        PostalCodeID = c.Int(nullable: false),
                        IsDefault = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .Index(t => t.PartnerID, unique: true, name: "PartnerAddress_PartnerID")
                .Index(t => t.Address, unique: true, name: "PartnerAddress_Address");
            
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
                "TMS.PartnerRole",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        PartnerRoleCode = c.String(nullable: false, maxLength: 10),
                        PartnerRoleDescription = c.String(),
                    })
                .PrimaryKey(t => t.ID)
                .Index(t => t.PartnerRoleCode, unique: true, name: "PartnerRole_PartnerRoleCode");
            
            CreateTable(
                "TMS.Partner",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        OrderPointTypeID = c.Int(nullable: false),
                        OrderPointCode = c.String(maxLength: 10),
                        PartnerNo = c.String(maxLength: 10),
                        PartnerName = c.String(),
                        PostalCodeID = c.Int(nullable: false),
                        PartnerTypeID = c.Int(nullable: false),
                        PartnerInitial = c.String(),
                        PartnerEmail = c.String(),
                    })
                .PrimaryKey(t => t.ID)
                .Index(t => t.OrderPointCode, unique: true, name: "Partner_OrderPointCode")
                .Index(t => t.PartnerNo, unique: true, name: "Partner_PartnerNo");
            
            CreateTable(
                "TMS.PartnerTypeFunction",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        PartnerID = c.Int(nullable: false),
                        PartnerTypeID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .Index(t => t.PartnerID, unique: true, name: "PartnerTypeFunction_PartnerID")
                .Index(t => t.PartnerTypeID, unique: true, name: "PartnerTypeFunction_PartnerTypeID");
            
            CreateTable(
                "TMS.PartnerType",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        PartnerTypeCode = c.String(nullable: false, maxLength: 10),
                        PartnerTypeDescription = c.String(),
                    })
                .PrimaryKey(t => t.ID)
                .Index(t => t.PartnerTypeCode, unique: true, name: "PartnerType_PartnerTypeCode");
            
            CreateTable(
                "TMS.PostalCode",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        PostalCodeNo = c.String(nullable: false, maxLength: 6),
                        SubDistrictID = c.Int(nullable: false),
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
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("TMS.City", t => t.CityID, cascadeDelete: true)
                .Index(t => t.SubdistrictCode, unique: true, name: "SubDistrict_SubdistrictCode")
                .Index(t => t.CityID);
            
            CreateTable(
                "TMS.Role",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        RoleCode = c.String(nullable: false, maxLength: 4),
                        RoleDescription = c.String(maxLength: 30),
                        ValidFrom = c.DateTime(nullable: false),
                        ValidTo = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .Index(t => t.RoleCode, unique: true, name: "Role_RoleCode");
            
            CreateTable(
                "TMS.TermsOfPayment",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        TermsOfPaymentCode = c.String(maxLength: 10),
                        TermsOfPaymentDescription = c.String(),
                    })
                .PrimaryKey(t => t.ID)
                .Index(t => t.TermsOfPaymentCode, unique: true, name: "TermsOfPayment_TermsOfPaymentCode");
            
            CreateTable(
                "TMS.TripDetail",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        TripHeaderID = c.Int(nullable: false),
                        ItemNo = c.Int(nullable: false),
                        OrderDetailID = c.Int(nullable: false),
                        Berat = c.Decimal(nullable: false, precision: 18, scale: 2),
                        KM = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Price = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.ID)
                .Index(t => t.TripHeaderID, unique: true, name: "TripDetail_TripHeaderID")
                .Index(t => t.ItemNo, unique: true, name: "TripDetail_ItemNo");
            
            CreateTable(
                "TMS.TripHeader",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        BusinessAreaID = c.Int(nullable: false),
                        TripNo = c.String(maxLength: 10),
                        TripStatusID = c.Int(nullable: false),
                        TripTypeID = c.Int(nullable: false),
                        TripDate = c.DateTime(nullable: false),
                        Notes = c.String(),
                        EstimatedTripTime = c.DateTime(nullable: false),
                        ActualTripTime = c.DateTime(nullable: false),
                        TotalWeight = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalKM = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ReferenceTripNo = c.Int(nullable: false),
                        IsParent = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .Index(t => t.TripNo, unique: true, name: "TripHeader_TripNo");
            
            CreateTable(
                "TMS.TripPartnerDetail",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        TripDetailID = c.Int(nullable: false),
                        PartnerID = c.Int(nullable: false),
                        PartnerTypeID = c.Int(nullable: false),
                        ParentID = c.Int(nullable: false),
                        IsParent = c.Int(nullable: false),
                        IsOriginal = c.Int(nullable: false),
                        DriverID = c.Int(nullable: false),
                        VehicleTypeID = c.Int(nullable: false),
                        Price = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.ID)
                .Index(t => t.TripDetailID, unique: true, name: "TripPartnerDetail_TripDetailID")
                .Index(t => t.PartnerID, unique: true, name: "TripPartnerDetail_PartnerID")
                .Index(t => t.PartnerTypeID, unique: true, name: "TripPartnerDetail_PartnerTypeID");
            
            CreateTable(
                "TMS.TripStatus",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        TripStatusCode = c.String(maxLength: 10),
                        TripStatusDescription = c.String(),
                    })
                .PrimaryKey(t => t.ID)
                .Index(t => t.TripStatusCode, unique: true, name: "TripStatus_TripStatusCode");
            
            CreateTable(
                "TMS.TripStatusHistory",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        TripHeaderID = c.Int(nullable: false),
                        TripStatusID = c.Int(nullable: false),
                        StepNo = c.Int(nullable: false),
                        IsOptional = c.Int(nullable: false),
                        CreatedTime = c.DateTime(nullable: false),
                        CreatedBy = c.DateTime(nullable: false),
                        LastModififiedTime = c.DateTime(nullable: false),
                        LastModifiedBy = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .Index(t => t.TripHeaderID, unique: true, name: "TripStatusHistory_TripHeaderID")
                .Index(t => t.TripStatusID, unique: true, name: "TripStatusHistory_TripStatusID")
                .Index(t => t.StepNo, unique: true, name: "TripStatusHistory_StepNo");
            
            CreateTable(
                "TMS.TripType",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        TripTypeCode = c.String(maxLength: 10),
                        TripTypeDescription = c.String(),
                    })
                .PrimaryKey(t => t.ID)
                .Index(t => t.TripTypeCode, unique: true, name: "TripType_TripTypeCode");
            
            CreateTable(
                "TMS.VehicleType",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        VehicleTypeCode = c.String(maxLength: 10),
                        VehicleTypeDescription = c.String(),
                    })
                .PrimaryKey(t => t.ID)
                .Index(t => t.VehicleTypeCode, unique: true, name: "VehicleType_VehicleTypeCode");
            
        }
        
        public override void Down()
        {
            DropForeignKey("TMS.PostalCode", "SubDistrictID", "TMS.SubDistrict");
            DropForeignKey("TMS.SubDistrict", "CityID", "TMS.City");
            DropForeignKey("TMS.MenuActivity", "MenuID", "TMS.Menu");
            DropForeignKey("TMS.MenuActivity", "ActivityID", "TMS.Activity");
            DropForeignKey("TMS.City", "ProvinceID", "TMS.Province");
            DropIndex("TMS.VehicleType", "VehicleType_VehicleTypeCode");
            DropIndex("TMS.TripType", "TripType_TripTypeCode");
            DropIndex("TMS.TripStatusHistory", "TripStatusHistory_StepNo");
            DropIndex("TMS.TripStatusHistory", "TripStatusHistory_TripStatusID");
            DropIndex("TMS.TripStatusHistory", "TripStatusHistory_TripHeaderID");
            DropIndex("TMS.TripStatus", "TripStatus_TripStatusCode");
            DropIndex("TMS.TripPartnerDetail", "TripPartnerDetail_PartnerTypeID");
            DropIndex("TMS.TripPartnerDetail", "TripPartnerDetail_PartnerID");
            DropIndex("TMS.TripPartnerDetail", "TripPartnerDetail_TripDetailID");
            DropIndex("TMS.TripHeader", "TripHeader_TripNo");
            DropIndex("TMS.TripDetail", "TripDetail_ItemNo");
            DropIndex("TMS.TripDetail", "TripDetail_TripHeaderID");
            DropIndex("TMS.TermsOfPayment", "TermsOfPayment_TermsOfPaymentCode");
            DropIndex("TMS.Role", "Role_RoleCode");
            DropIndex("TMS.SubDistrict", new[] { "CityID" });
            DropIndex("TMS.SubDistrict", "SubDistrict_SubdistrictCode");
            DropIndex("TMS.PostalCode", new[] { "SubDistrictID" });
            DropIndex("TMS.PostalCode", "PostalCode_PostalCodeNo");
            DropIndex("TMS.PartnerType", "PartnerType_PartnerTypeCode");
            DropIndex("TMS.PartnerTypeFunction", "PartnerTypeFunction_PartnerTypeID");
            DropIndex("TMS.PartnerTypeFunction", "PartnerTypeFunction_PartnerID");
            DropIndex("TMS.Partner", "Partner_PartnerNo");
            DropIndex("TMS.Partner", "Partner_OrderPointCode");
            DropIndex("TMS.PartnerRole", "PartnerRole_PartnerRoleCode");
            DropIndex("TMS.PartnerPIC", "PartnerPIC_PICID");
            DropIndex("TMS.PartnerPIC", "PartnerPIC_PartnerID");
            DropIndex("TMS.PartnerAddress", "PartnerAddress_Address");
            DropIndex("TMS.PartnerAddress", "PartnerAddress_PartnerID");
            DropIndex("TMS.OrderTypeStatusWorkFlow", "OrderTypeStatusWorkFlow_StepNo");
            DropIndex("TMS.OrderTypeStatusWorkFlow", "OrderTypeStatusWorkFlow_OrderStatus");
            DropIndex("TMS.OrderTypeStatusWorkFlow", "OrderTypeStatusWorkFlow_TipeOrderID");
            DropIndex("TMS.OrderType", "OrderType_OrderTypeCode");
            DropIndex("TMS.OrderTripStatusWorkFlow", "OrderTripStatusWorkFlow_StepNo");
            DropIndex("TMS.OrderTripStatusWorkFlow", "OrderTripStatusWorkFlow_TripStatusID");
            DropIndex("TMS.OrderTripStatusWorkFlow", "OrderTripStatusWorkFlow_TripTypeID");
            DropIndex("TMS.OrderStatusHistory", "OrderStatusHistory_StepNo");
            DropIndex("TMS.OrderStatusHistory", "OrderStatusHistory_OrderStatus");
            DropIndex("TMS.OrderStatusHistory", "OrderStatusHistory_OrderHeaderID");
            DropIndex("TMS.OrderPointType", "OrderPointType_PartnerRoleCode");
            DropIndex("TMS.OrderPartnerDetail", "OrderPartnerDetail_PartnerID");
            DropIndex("TMS.OrderPartnerDetail", "OrderPartnerDetail_PartnerTypeID");
            DropIndex("TMS.OrderPartnerDetail", "OrderPartnerDetail_OrderDetailID");
            DropIndex("TMS.OrderHeader", "OrderHeader_OrderNo");
            DropIndex("TMS.OrderHeader", "OrderHeader_TipeOrder");
            DropIndex("TMS.OrderHeaderHSOAdditionalData", "OrderHeaderHSOAdditionalData_OrderHeaderID");
            DropIndex("TMS.OrderDetail", "OrderDetail_ItemNo");
            DropIndex("TMS.OrderDetail", "OrderDetail_OrderHeaderID");
            DropIndex("TMS.NumberingRange", "NumberingRange_TransactionTypeCode");
            DropIndex("TMS.NumberingRange", "NumberingRange_BusinessAreaCode");
            DropIndex("TMS.NumberingRange", "NumberingRange_CompanyCodeCode");
            DropIndex("TMS.Menu", "Menu_MenuCode");
            DropIndex("TMS.MenuActivity", new[] { "ActivityID" });
            DropIndex("TMS.MenuActivity", new[] { "MenuID" });
            DropIndex("TMS.MappingOrderPartner", "MappingOrderPartner_PartnerTypeID");
            DropIndex("TMS.MappingOrderPartner", "MappingOrderPartner_OrderTypeID");
            DropIndex("TMS.Driver", "Driver_DrivingLicenseNo");
            DropIndex("TMS.Driver", "Driver_IdentityNo");
            DropIndex("TMS.Driver", "Driver_DriverNo");
            DropIndex("TMS.CompanyCode", "CompanyCode_CompanyCodeCode");
            DropIndex("TMS.CompanyCodeNumberingRange", "CompanyCodeNumberingRange_TransactionTypeCode");
            DropIndex("TMS.CompanyCodeNumberingRange", "CompanyCodeNumberingRange_BusinessAreaCode");
            DropIndex("TMS.CompanyCodeNumberingRange", "CompanyCodeNumberingRange_CompanyCodeCode");
            DropIndex("TMS.Province", "Province_ProvinceCode");
            DropIndex("TMS.City", new[] { "ProvinceID" });
            DropIndex("TMS.City", "City_CityCode");
            DropIndex("TMS.CancellationReason", "CancellationReason_CancellationReasonCode");
            DropIndex("TMS.BusinessArea", "BusinessArea_CompanyCodeID");
            DropIndex("TMS.BusinessArea", "BusinessArea_BusinessAreaCode");
            DropIndex("TMS.BillingTrip", "BillingTrip_BillingNo");
            DropIndex("TMS.BillingTrip", "BillingTrip_BusinessAreaID");
            DropIndex("TMS.BillingTrip", "BillingTrip_CompanyCodeID");
            DropIndex("TMS.BillingTripPartnerDetail", "BillingTripPartnerDetail_PartnerID");
            DropIndex("TMS.BillingTripPartnerDetail", "BillingTripPartnerDetail_PartnerTypeID");
            DropIndex("TMS.BillingTripPartnerDetail", "BillingTripPartnerDetail_BillingDetailID");
            DropIndex("TMS.BillingTripDetail", "BillingTripDetail_TripDetailID");
            DropIndex("TMS.BillingTripDetail", "BillingTripDetail_ItemNo");
            DropIndex("TMS.BillingTripDetail", "BillingTripDetail_BillingHeaderID");
            DropIndex("TMS.Application", "Application_ApplicationCode");
            DropIndex("TMS.Activity", "Activity_ActivityCode");
            DropTable("TMS.VehicleType");
            DropTable("TMS.TripType");
            DropTable("TMS.TripStatusHistory");
            DropTable("TMS.TripStatus");
            DropTable("TMS.TripPartnerDetail");
            DropTable("TMS.TripHeader");
            DropTable("TMS.TripDetail");
            DropTable("TMS.TermsOfPayment");
            DropTable("TMS.Role");
            DropTable("TMS.SubDistrict");
            DropTable("TMS.PostalCode");
            DropTable("TMS.PartnerType");
            DropTable("TMS.PartnerTypeFunction");
            DropTable("TMS.Partner");
            DropTable("TMS.PartnerRole");
            DropTable("TMS.PartnerPIC");
            DropTable("TMS.PartnerAddress");
            DropTable("TMS.OrderTypeStatusWorkFlow");
            DropTable("TMS.OrderType");
            DropTable("TMS.OrderTripStatusWorkFlow");
            DropTable("TMS.OrderStatusHistory");
            DropTable("TMS.OrderPointType");
            DropTable("TMS.OrderPartnerDetail");
            DropTable("TMS.OrderHeader");
            DropTable("TMS.OrderHeaderHSOAdditionalData");
            DropTable("TMS.OrderDetail");
            DropTable("TMS.NumberingRange");
            DropTable("TMS.Menu");
            DropTable("TMS.MenuActivity");
            DropTable("TMS.MappingOrderPartner");
            DropTable("TMS.Driver");
            DropTable("TMS.CompanyCode");
            DropTable("TMS.CompanyCodeNumberingRange");
            DropTable("TMS.Province");
            DropTable("TMS.City");
            DropTable("TMS.CancellationReason");
            DropTable("TMS.BusinessArea");
            DropTable("TMS.BillingTrip");
            DropTable("TMS.BillingTripPartnerDetail");
            DropTable("TMS.BillingTripDetail");
            DropTable("TMS.Application");
            DropTable("TMS.Activity");
        }
    }
}
