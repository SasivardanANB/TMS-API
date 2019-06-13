namespace DMS.DataGateway.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DeviceTokenTableAdded : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "DMS.DeviceToken",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        DriverId = c.Int(nullable: false),
                        DeviceKey = c.String(maxLength: 250),
                        CreatedBy = c.String(maxLength: 100),
                        CreatedTime = c.DateTime(),
                        LastModifiedBy = c.String(maxLength: 100),
                        LastModifiedTime = c.DateTime(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("DMS.Driver", t => t.DriverId, cascadeDelete: true)
                .Index(t => t.DriverId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("DMS.DeviceToken", "DriverId", "DMS.Driver");
            DropIndex("DMS.DeviceToken", new[] { "DriverId" });
            DropTable("DMS.DeviceToken");
        }
    }
}
