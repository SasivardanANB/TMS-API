namespace DMS.DataGateway.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DMSDB_v10 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "DMS.BusinessArea",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        BusinessAreaCode = c.String(nullable: false, maxLength: 4),
                        BusinessAreaDescription = c.String(maxLength: 100),
                        Address = c.String(maxLength: 200),
                        PostalCodeID = c.Int(),
                        CreatedBy = c.String(maxLength: 100),
                        CreatedTime = c.DateTime(),
                        LastModifiedBy = c.String(maxLength: 100),
                        LastModifiedTime = c.DateTime(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("DMS.PostalCode", t => t.PostalCodeID)
                .Index(t => t.BusinessAreaCode, unique: true, name: "BusinessArea_BusinessAreaCode")
                .Index(t => t.PostalCodeID);
            
            CreateTable(
                "DMS.PostalCode",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        PostalCodeNo = c.String(nullable: false, maxLength: 6),
                        SubDistrictID = c.Int(nullable: false),
                        CreatedBy = c.String(maxLength: 100),
                        CreatedTime = c.DateTime(),
                        LastModifiedBy = c.String(maxLength: 100),
                        LastModifiedTime = c.DateTime(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("DMS.SubDistrict", t => t.SubDistrictID, cascadeDelete: true)
                .Index(t => t.PostalCodeNo, unique: true, name: "PostalCode_PostalCodeNo")
                .Index(t => t.SubDistrictID);
            
            CreateTable(
                "DMS.SubDistrict",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        SubDistrictCode = c.String(nullable: false, maxLength: 4),
                        SubDistrictName = c.String(maxLength: 50),
                        CityID = c.Int(nullable: false),
                        CreatedBy = c.String(maxLength: 100),
                        CreatedTime = c.DateTime(),
                        LastModifiedBy = c.String(maxLength: 100),
                        LastModifiedTime = c.DateTime(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("DMS.City", t => t.CityID, cascadeDelete: true)
                .Index(t => t.SubDistrictCode, unique: true, name: "SubDistrict_SubDistrictCode")
                .Index(t => t.CityID);
            
            CreateTable(
                "DMS.City",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        CityCode = c.String(nullable: false, maxLength: 4),
                        CityDescription = c.String(maxLength: 50),
                        ProvinceID = c.Int(nullable: false),
                        CreatedBy = c.String(maxLength: 100),
                        CreatedTime = c.DateTime(),
                        LastModifiedBy = c.String(maxLength: 100),
                        LastModifiedTime = c.DateTime(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("DMS.Province", t => t.ProvinceID, cascadeDelete: true)
                .Index(t => t.CityCode, unique: true, name: "City_CityCode")
                .Index(t => t.ProvinceID);
            
            CreateTable(
                "DMS.Province",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        ProvinceCode = c.String(maxLength: 4),
                        ProvinceDescription = c.String(maxLength: 50),
                        CreatedBy = c.String(maxLength: 100),
                        CreatedTime = c.DateTime(),
                        LastModifiedBy = c.String(maxLength: 100),
                        LastModifiedTime = c.DateTime(),
                    })
                .PrimaryKey(t => t.ID)
                .Index(t => t.ProvinceCode, unique: true, name: "Province_ProvinceCode");
            
            CreateTable(
                "DMS.Driver",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        DriverNo = c.String(maxLength: 12),
                        UserName = c.String(nullable: false, maxLength: 30),
                        Password = c.String(maxLength: 30),
                        IsActive = c.Boolean(nullable: false),
                        FirstName = c.String(maxLength: 30),
                        LastName = c.String(maxLength: 30),
                        Email = c.String(maxLength: 50),
                        PhoneNumber = c.String(maxLength: 15),
                        PICName = c.String(maxLength: 50),
                        PICPhone = c.String(maxLength: 15),
                        PICEmail = c.String(maxLength: 50),
                        CreatedBy = c.String(maxLength: 100),
                        CreatedTime = c.DateTime(),
                        LastModifiedBy = c.String(maxLength: 100),
                        LastModifiedTime = c.DateTime(),
                    })
                .PrimaryKey(t => t.ID)
                .Index(t => t.DriverNo, unique: true, name: "Driver_DriverNo");
            
            CreateTable(
                "DMS.ImageGuid",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        ImageGuIdValue = c.String(maxLength: 1000),
                        IsActive = c.Boolean(nullable: false),
                        CreatedBy = c.String(maxLength: 100),
                        CreatedTime = c.DateTime(),
                        LastModifiedBy = c.String(maxLength: 100),
                        LastModifiedTime = c.DateTime(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "DMS.ImageType",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        ImageTypeCode = c.Int(nullable: false),
                        ImageTypeDescription = c.String(),
                        CreatedBy = c.String(maxLength: 100),
                        CreatedTime = c.DateTime(),
                        LastModifiedBy = c.String(maxLength: 100),
                        LastModifiedTime = c.DateTime(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "DMS.PartnerPartnerType",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        PartnerId = c.Int(nullable: false),
                        PartnerTypeId = c.Int(nullable: false),
                        CreatedBy = c.String(maxLength: 100),
                        CreatedTime = c.DateTime(),
                        LastModifiedBy = c.String(maxLength: 100),
                        LastModifiedTime = c.DateTime(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("DMS.Partner", t => t.PartnerId, cascadeDelete: true)
                .ForeignKey("DMS.PartnerType", t => t.PartnerTypeId, cascadeDelete: true)
                .Index(t => t.PartnerId)
                .Index(t => t.PartnerTypeId);
            
            CreateTable(
                "DMS.Partner",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        PostalCodeId = c.Int(nullable: false),
                        PartnerNo = c.String(maxLength: 10),
                        PartnerName = c.String(maxLength: 30),
                        IsActive = c.Boolean(nullable: false),
                        CreatedBy = c.String(maxLength: 100),
                        CreatedTime = c.DateTime(),
                        LastModifiedBy = c.String(maxLength: 100),
                        LastModifiedTime = c.DateTime(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("DMS.PostalCode", t => t.PostalCodeId, cascadeDelete: true)
                .Index(t => t.PostalCodeId)
                .Index(t => t.PartnerNo, unique: true, name: "Partner_PartnerNo");
            
            CreateTable(
                "DMS.PartnerType",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        PartnerTypeCode = c.String(nullable: false, maxLength: 10),
                        PartnerTypeDescription = c.String(maxLength: 50),
                        CreatedBy = c.String(maxLength: 100),
                        CreatedTime = c.DateTime(),
                        LastModifiedBy = c.String(maxLength: 100),
                        LastModifiedTime = c.DateTime(),
                    })
                .PrimaryKey(t => t.ID)
                .Index(t => t.PartnerTypeCode, unique: true, name: "PartnerType_PartnerTypeCode");
            
            CreateTable(
                "DMS.ShipmentList",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        NumberOfBoxes = c.Int(nullable: false),
                        Note = c.String(maxLength: 200),
                        PackingSheetNumber = c.String(maxLength: 20),
                        StopPointId = c.Int(nullable: false),
                        CreatedBy = c.String(maxLength: 100),
                        CreatedTime = c.DateTime(),
                        LastModifiedBy = c.String(maxLength: 100),
                        LastModifiedTime = c.DateTime(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("DMS.TripDetail", t => t.StopPointId, cascadeDelete: true)
                .Index(t => t.StopPointId);
            
            CreateTable(
                "DMS.TripDetail",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        TripID = c.Int(nullable: false),
                        PartnerId = c.Int(nullable: false),
                        SequenceNumber = c.Int(nullable: false),
                        ActualDeliveryDate = c.DateTime(nullable: false),
                        EstimatedDeliveryDate = c.DateTime(nullable: false),
                        CreatedBy = c.String(maxLength: 100),
                        CreatedTime = c.DateTime(),
                        LastModifiedBy = c.String(maxLength: 100),
                        LastModifiedTime = c.DateTime(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("DMS.Partner", t => t.PartnerId, cascadeDelete: true)
                .ForeignKey("DMS.TripHeader", t => t.TripID, cascadeDelete: true)
                .Index(t => t.TripID)
                .Index(t => t.PartnerId);
            
            CreateTable(
                "DMS.TripHeader",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        TripNumber = c.String(maxLength: 50),
                        OrderNumber = c.String(maxLength: 15),
                        TransporterName = c.String(maxLength: 50),
                        TransporterCode = c.String(),
                        DriverId = c.Int(nullable: false),
                        VehicleType = c.String(),
                        VehicleNumber = c.String(maxLength: 12),
                        TripType = c.String(),
                        Weight = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PoliceNumber = c.String(maxLength: 12),
                        CurrentTripStatusId = c.Int(),
                        OrderType = c.Int(nullable: false),
                        TripDate = c.DateTime(nullable: false),
                        BusinessAreaId = c.Int(nullable: false),
                        ShipmentScheduleImageID = c.Int(),
                        CreatedBy = c.String(maxLength: 100),
                        CreatedTime = c.DateTime(),
                        LastModifiedBy = c.String(maxLength: 100),
                        LastModifiedTime = c.DateTime(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("DMS.BusinessArea", t => t.BusinessAreaId, cascadeDelete: true)
                .ForeignKey("DMS.Driver", t => t.DriverId, cascadeDelete: true)
                .ForeignKey("DMS.ImageGuid", t => t.ShipmentScheduleImageID)
                .ForeignKey("DMS.TripStatus", t => t.CurrentTripStatusId)
                .Index(t => t.TripNumber, unique: true, name: "TripNumber")
                .Index(t => t.DriverId)
                .Index(t => t.CurrentTripStatusId)
                .Index(t => t.BusinessAreaId)
                .Index(t => t.ShipmentScheduleImageID);
            
            CreateTable(
                "DMS.TripStatus",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        StatusCode = c.String(maxLength: 4),
                        StatusName = c.String(maxLength: 30),
                        CreatedBy = c.String(maxLength: 100),
                        CreatedTime = c.DateTime(),
                        LastModifiedBy = c.String(maxLength: 100),
                        LastModifiedTime = c.DateTime(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "DMS.StopPointImages",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        StopPointId = c.Int(nullable: false),
                        ImageId = c.Int(nullable: false),
                        ImageTypeId = c.Int(nullable: false),
                        CreatedBy = c.String(maxLength: 100),
                        CreatedTime = c.DateTime(),
                        LastModifiedBy = c.String(maxLength: 100),
                        LastModifiedTime = c.DateTime(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("DMS.ImageGuid", t => t.ImageId, cascadeDelete: true)
                .ForeignKey("DMS.ImageType", t => t.ImageTypeId, cascadeDelete: true)
                .ForeignKey("DMS.TripDetail", t => t.StopPointId, cascadeDelete: true)
                .Index(t => t.StopPointId)
                .Index(t => t.ImageId)
                .Index(t => t.ImageTypeId);
            
            CreateTable(
                "DMS.TokenManager",
                c => new
                    {
                        TokenID = c.Int(nullable: false, identity: true),
                        TokenKey = c.String(maxLength: 200),
                        IssuedOn = c.DateTime(nullable: false),
                        ExpiresOn = c.DateTime(nullable: false),
                        CreatedOn = c.DateTime(nullable: false),
                        DriverId = c.Int(nullable: false),
                        CreatedBy = c.String(maxLength: 100),
                        CreatedTime = c.DateTime(),
                        LastModifiedBy = c.String(maxLength: 100),
                        LastModifiedTime = c.DateTime(),
                    })
                .PrimaryKey(t => t.TokenID)
                .ForeignKey("DMS.Driver", t => t.DriverId, cascadeDelete: true)
                .Index(t => t.DriverId);
            
            CreateTable(
                "DMS.TripStatusHistory",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        StopPointId = c.Int(nullable: false),
                        StatusDate = c.DateTime(nullable: false),
                        Remarks = c.String(maxLength: 200),
                        TripStatusId = c.Int(nullable: false),
                        CreatedBy = c.String(maxLength: 100),
                        CreatedTime = c.DateTime(),
                        LastModifiedBy = c.String(maxLength: 100),
                        LastModifiedTime = c.DateTime(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("DMS.TripDetail", t => t.StopPointId, cascadeDelete: true)
                .ForeignKey("DMS.TripStatus", t => t.TripStatusId, cascadeDelete: true)
                .Index(t => t.StopPointId)
                .Index(t => t.TripStatusId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("DMS.TripStatusHistory", "TripStatusId", "DMS.TripStatus");
            DropForeignKey("DMS.TripStatusHistory", "StopPointId", "DMS.TripDetail");
            DropForeignKey("DMS.TokenManager", "DriverId", "DMS.Driver");
            DropForeignKey("DMS.StopPointImages", "StopPointId", "DMS.TripDetail");
            DropForeignKey("DMS.StopPointImages", "ImageTypeId", "DMS.ImageType");
            DropForeignKey("DMS.StopPointImages", "ImageId", "DMS.ImageGuid");
            DropForeignKey("DMS.ShipmentList", "StopPointId", "DMS.TripDetail");
            DropForeignKey("DMS.TripDetail", "TripID", "DMS.TripHeader");
            DropForeignKey("DMS.TripHeader", "CurrentTripStatusId", "DMS.TripStatus");
            DropForeignKey("DMS.TripHeader", "ShipmentScheduleImageID", "DMS.ImageGuid");
            DropForeignKey("DMS.TripHeader", "DriverId", "DMS.Driver");
            DropForeignKey("DMS.TripHeader", "BusinessAreaId", "DMS.BusinessArea");
            DropForeignKey("DMS.TripDetail", "PartnerId", "DMS.Partner");
            DropForeignKey("DMS.PartnerPartnerType", "PartnerTypeId", "DMS.PartnerType");
            DropForeignKey("DMS.PartnerPartnerType", "PartnerId", "DMS.Partner");
            DropForeignKey("DMS.Partner", "PostalCodeId", "DMS.PostalCode");
            DropForeignKey("DMS.BusinessArea", "PostalCodeID", "DMS.PostalCode");
            DropForeignKey("DMS.PostalCode", "SubDistrictID", "DMS.SubDistrict");
            DropForeignKey("DMS.SubDistrict", "CityID", "DMS.City");
            DropForeignKey("DMS.City", "ProvinceID", "DMS.Province");
            DropIndex("DMS.TripStatusHistory", new[] { "TripStatusId" });
            DropIndex("DMS.TripStatusHistory", new[] { "StopPointId" });
            DropIndex("DMS.TokenManager", new[] { "DriverId" });
            DropIndex("DMS.StopPointImages", new[] { "ImageTypeId" });
            DropIndex("DMS.StopPointImages", new[] { "ImageId" });
            DropIndex("DMS.StopPointImages", new[] { "StopPointId" });
            DropIndex("DMS.TripHeader", new[] { "ShipmentScheduleImageID" });
            DropIndex("DMS.TripHeader", new[] { "BusinessAreaId" });
            DropIndex("DMS.TripHeader", new[] { "CurrentTripStatusId" });
            DropIndex("DMS.TripHeader", new[] { "DriverId" });
            DropIndex("DMS.TripHeader", "TripNumber");
            DropIndex("DMS.TripDetail", new[] { "PartnerId" });
            DropIndex("DMS.TripDetail", new[] { "TripID" });
            DropIndex("DMS.ShipmentList", new[] { "StopPointId" });
            DropIndex("DMS.PartnerType", "PartnerType_PartnerTypeCode");
            DropIndex("DMS.Partner", "Partner_PartnerNo");
            DropIndex("DMS.Partner", new[] { "PostalCodeId" });
            DropIndex("DMS.PartnerPartnerType", new[] { "PartnerTypeId" });
            DropIndex("DMS.PartnerPartnerType", new[] { "PartnerId" });
            DropIndex("DMS.Driver", "Driver_DriverNo");
            DropIndex("DMS.Province", "Province_ProvinceCode");
            DropIndex("DMS.City", new[] { "ProvinceID" });
            DropIndex("DMS.City", "City_CityCode");
            DropIndex("DMS.SubDistrict", new[] { "CityID" });
            DropIndex("DMS.SubDistrict", "SubDistrict_SubDistrictCode");
            DropIndex("DMS.PostalCode", new[] { "SubDistrictID" });
            DropIndex("DMS.PostalCode", "PostalCode_PostalCodeNo");
            DropIndex("DMS.BusinessArea", new[] { "PostalCodeID" });
            DropIndex("DMS.BusinessArea", "BusinessArea_BusinessAreaCode");
            DropTable("DMS.TripStatusHistory");
            DropTable("DMS.TokenManager");
            DropTable("DMS.StopPointImages");
            DropTable("DMS.TripStatus");
            DropTable("DMS.TripHeader");
            DropTable("DMS.TripDetail");
            DropTable("DMS.ShipmentList");
            DropTable("DMS.PartnerType");
            DropTable("DMS.Partner");
            DropTable("DMS.PartnerPartnerType");
            DropTable("DMS.ImageType");
            DropTable("DMS.ImageGuid");
            DropTable("DMS.Driver");
            DropTable("DMS.Province");
            DropTable("DMS.City");
            DropTable("DMS.SubDistrict");
            DropTable("DMS.PostalCode");
            DropTable("DMS.BusinessArea");
        }
    }
}
