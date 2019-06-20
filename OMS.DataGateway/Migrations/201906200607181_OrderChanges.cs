namespace OMS.DataGateway.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class OrderChanges : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "OMS.ImageGuid",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        ImageGuIdValue = c.String(maxLength: 1000),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            AddColumn("OMS.OrderDetail", "Katerangan", c => c.String());
            AddColumn("OMS.OrderHeader", "SOPONumber", c => c.String(maxLength: 10));
            AddColumn("OMS.OrderHeader", "Harga", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("OMS.OrderHeader", "ShipmentScheduleImageID", c => c.Int());
            AddColumn("OMS.OrderHeader", "UploadType", c => c.Int(nullable: false));
            CreateIndex("OMS.OrderHeader", "ShipmentScheduleImageID");
            AddForeignKey("OMS.OrderHeader", "ShipmentScheduleImageID", "OMS.ImageGuid", "ID");
        }
        
        public override void Down()
        {
            DropForeignKey("OMS.OrderHeader", "ShipmentScheduleImageID", "OMS.ImageGuid");
            DropIndex("OMS.OrderHeader", new[] { "ShipmentScheduleImageID" });
            DropColumn("OMS.OrderHeader", "UploadType");
            DropColumn("OMS.OrderHeader", "ShipmentScheduleImageID");
            DropColumn("OMS.OrderHeader", "Harga");
            DropColumn("OMS.OrderHeader", "SOPONumber");
            DropColumn("OMS.OrderDetail", "Katerangan");
            DropTable("OMS.ImageGuid");
        }
    }
}
