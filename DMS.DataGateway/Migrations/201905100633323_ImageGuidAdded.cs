namespace DMS.DataGateway.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ImageGuidAdded : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "DMS.ImageGuid",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        ImageGuIdValue = c.String(maxLength: 1000),
                        CreatedBy = c.String(defaultValue:"SYSTEM"),
                        CreatedTime = c.DateTime(nullable: false, defaultValueSql:"GETDATE()"),
                        LastModifiedBy = c.String(),
                        LastModifiedTime = c.DateTime(),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "DMS.POD",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        TripNo = c.String(nullable: false, maxLength: 25),
                        SourcePointId = c.Int(nullable: false),
                        DestinationPointId = c.Int(nullable: false),
                        OrderTypeId = c.Int(nullable: false),
                        ShipmentNo = c.String(nullable: false, maxLength: 25),
                        DoosNo = c.String(nullable: false, maxLength: 20),
                        MaterialNo = c.String(nullable: false, maxLength: 20),
                        MaterialDescription = c.String(nullable: false, maxLength: 100),
                        Weight = c.String(nullable: false, maxLength: 7),
                        PoliceNo = c.String(nullable: false, maxLength: 12),
                        TripStatusId = c.Int(nullable: false),
                        PODImageId = c.Int(nullable: false),
                        CreatedTime = c.DateTime(nullable: false, defaultValueSql: "GETDATE()"),
                        LastModifiedBy = c.String(),
                        LastModifiedTime = c.DateTime(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "DMS.TripGuid",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        TripEventLogID = c.Int(nullable: false),
                        ImageID = c.Int(nullable: false),
                        CreatedBy = c.String(defaultValue: "SYSTEM"),
                        CreatedTime = c.DateTime(nullable: false, defaultValueSql: "GETDATE()"),
                        LastModifiedBy = c.String(),
                        LastModifiedTime = c.DateTime(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("DMS.ImageGuid", t => t.ImageID, cascadeDelete: true)
                .ForeignKey("DMS.TripEventLog", t => t.TripEventLogID, cascadeDelete: true)
                .Index(t => t.TripEventLogID)
                .Index(t => t.ImageID);
            
            AddColumn("DMS.User", "CreatedTime", c => c.DateTime(nullable: false));
            AddColumn("DMS.User", "LastModifiedBy", c => c.String());
            AddColumn("DMS.User", "LastModifiedTime", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropForeignKey("DMS.TripGuid", "TripEventLogID", "DMS.TripEventLog");
            DropForeignKey("DMS.TripGuid", "ImageID", "DMS.ImageGuid");
            DropIndex("DMS.TripGuid", new[] { "ImageID" });
            DropIndex("DMS.TripGuid", new[] { "TripEventLogID" });
            DropColumn("DMS.User", "LastModifiedTime");
            DropColumn("DMS.User", "LastModifiedBy");
            DropColumn("DMS.User", "CreatedTime");
            DropTable("DMS.TripGuid");
            DropTable("DMS.POD");
            DropTable("DMS.ImageGuid");
        }
    }
}
