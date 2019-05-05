namespace DMS.DataGateway.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DMS_V10 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "DMS.City",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        CityCode = c.String(nullable: false, maxLength: 4),
                        CityDescription = c.String(maxLength: 50),
                        ProvinceID = c.Int(nullable: false),
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
                    })
                .PrimaryKey(t => t.ID)
                .Index(t => t.ProvinceCode, unique: true, name: "Province_ProvinceCode");
            
            CreateTable(
                "DMS.Location",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        TypeofLocation = c.String(),
                        CityId = c.Int(nullable: false),
                        Name = c.String(nullable: false, maxLength: 50),
                        Place = c.String(),
                        Address = c.String(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("DMS.City", t => t.CityId, cascadeDelete: true)
                .Index(t => t.CityId)
                .Index(t => t.Name, unique: true, name: "Location_Name");
            
            CreateTable(
                "DMS.PostalCode",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        PostalCodeNo = c.String(nullable: false, maxLength: 6),
                        SubDistrictID = c.Int(nullable: false),
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
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("DMS.City", t => t.CityID, cascadeDelete: true)
                .Index(t => t.SubDistrictCode, unique: true, name: "SubDistrict_SubDistrictCode")
                .Index(t => t.CityID);
            
            CreateTable(
                "DMS.ShipmentList",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        NumberOfBoxes = c.Int(nullable: false),
                        Note = c.String(),
                        PackingSheetNumber = c.String(),
                        StopPointId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("DMS.StopPoints", t => t.StopPointId, cascadeDelete: true)
                .Index(t => t.StopPointId);
            
            CreateTable(
                "DMS.StopPoints",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        TripID = c.Int(nullable: false),
                        LocationID = c.Int(nullable: false),
                        SequenceNumber = c.Int(nullable: false),
                        ActualDeliveryDate = c.DateTime(nullable: false),
                        EstimatedDeliveryDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("DMS.Location", t => t.LocationID, cascadeDelete: true)
                .ForeignKey("DMS.TripManager", t => t.TripID, cascadeDelete: true)
                .Index(t => t.TripID)
                .Index(t => t.LocationID);
            
            CreateTable(
                "DMS.TripManager",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        TripNumber = c.String(maxLength: 50),
                        OrderNumber = c.String(maxLength: 50),
                        TransporterName = c.String(maxLength: 50),
                        TransporterCode = c.String(),
                        UserId = c.Int(nullable: false),
                        VehicleType = c.String(),
                        VehicleNumber = c.String(),
                        TripType = c.String(),
                        Weight = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PoliceNumber = c.String(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("DMS.User", t => t.UserId, cascadeDelete: true)
                .Index(t => t.TripNumber, unique: true, name: "TripNumber")
                .Index(t => t.UserId);
            
            CreateTable(
                "DMS.User",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        UserName = c.String(nullable: false, maxLength: 50),
                        Password = c.String(),
                        IsActive = c.Boolean(nullable: false),
                        FirstName = c.String(),
                        LastName = c.String(),
                        Email = c.String(),
                        PhoneNumber = c.String(),
                    })
                .PrimaryKey(t => t.ID)
                .Index(t => t.UserName, unique: true, name: "User_UserName");
            
            CreateTable(
                "DMS.StopPointImages",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        StopPointId = c.Int(nullable: false),
                        ImageGUId = c.String(),
                        ImageType = c.String(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("DMS.StopPoints", t => t.StopPointId, cascadeDelete: true)
                .Index(t => t.StopPointId);
            
            CreateTable(
                "DMS.TokenManager",
                c => new
                    {
                        TokenID = c.Int(nullable: false, identity: true),
                        TokenKey = c.String(),
                        IssuedOn = c.DateTime(nullable: false),
                        ExpiresOn = c.DateTime(nullable: false),
                        CreatedOn = c.DateTime(nullable: false),
                        UserID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.TokenID)
                .ForeignKey("DMS.User", t => t.UserID, cascadeDelete: true)
                .Index(t => t.UserID);
            
            CreateTable(
                "DMS.TripStatus",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        StatusName = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "DMS.TripEventLog",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        StopPointId = c.Int(nullable: false),
                        StatusDate = c.DateTime(nullable: false),
                        Remarks = c.String(),
                        TripStatusId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("DMS.StopPoints", t => t.StopPointId, cascadeDelete: true)
                .ForeignKey("DMS.TripStatus", t => t.TripStatusId, cascadeDelete: true)
                .Index(t => t.StopPointId)
                .Index(t => t.TripStatusId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("DMS.TripEventLog", "TripStatusId", "DMS.TripStatus");
            DropForeignKey("DMS.TripEventLog", "StopPointId", "DMS.StopPoints");
            DropForeignKey("DMS.TokenManager", "UserID", "DMS.User");
            DropForeignKey("DMS.StopPointImages", "StopPointId", "DMS.StopPoints");
            DropForeignKey("DMS.ShipmentList", "StopPointId", "DMS.StopPoints");
            DropForeignKey("DMS.StopPoints", "TripID", "DMS.TripManager");
            DropForeignKey("DMS.TripManager", "UserId", "DMS.User");
            DropForeignKey("DMS.StopPoints", "LocationID", "DMS.Location");
            DropForeignKey("DMS.PostalCode", "SubDistrictID", "DMS.SubDistrict");
            DropForeignKey("DMS.SubDistrict", "CityID", "DMS.City");
            DropForeignKey("DMS.Location", "CityId", "DMS.City");
            DropForeignKey("DMS.City", "ProvinceID", "DMS.Province");
            DropIndex("DMS.TripEventLog", new[] { "TripStatusId" });
            DropIndex("DMS.TripEventLog", new[] { "StopPointId" });
            DropIndex("DMS.TokenManager", new[] { "UserID" });
            DropIndex("DMS.StopPointImages", new[] { "StopPointId" });
            DropIndex("DMS.User", "User_UserName");
            DropIndex("DMS.TripManager", new[] { "UserId" });
            DropIndex("DMS.TripManager", "TripNumber");
            DropIndex("DMS.StopPoints", new[] { "LocationID" });
            DropIndex("DMS.StopPoints", new[] { "TripID" });
            DropIndex("DMS.ShipmentList", new[] { "StopPointId" });
            DropIndex("DMS.SubDistrict", new[] { "CityID" });
            DropIndex("DMS.SubDistrict", "SubDistrict_SubDistrictCode");
            DropIndex("DMS.PostalCode", new[] { "SubDistrictID" });
            DropIndex("DMS.PostalCode", "PostalCode_PostalCodeNo");
            DropIndex("DMS.Location", "Location_Name");
            DropIndex("DMS.Location", new[] { "CityId" });
            DropIndex("DMS.Province", "Province_ProvinceCode");
            DropIndex("DMS.City", new[] { "ProvinceID" });
            DropIndex("DMS.City", "City_CityCode");
            DropTable("DMS.TripEventLog");
            DropTable("DMS.TripStatus");
            DropTable("DMS.TokenManager");
            DropTable("DMS.StopPointImages");
            DropTable("DMS.User");
            DropTable("DMS.TripManager");
            DropTable("DMS.StopPoints");
            DropTable("DMS.ShipmentList");
            DropTable("DMS.SubDistrict");
            DropTable("DMS.PostalCode");
            DropTable("DMS.Location");
            DropTable("DMS.Province");
            DropTable("DMS.City");
        }
    }
}
