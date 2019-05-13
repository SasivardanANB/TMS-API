namespace TMS.DataGateway.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class OrderHeaderNewColumns : DbMigration
    {
        public override void Up()
        {
            AddColumn("TMS.OrderHeader", "Harga", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("TMS.OrderHeader", "ShipmentScheduleImageID", c => c.Int());
            CreateIndex("TMS.OrderHeader", "ShipmentScheduleImageID");
            AddForeignKey("TMS.OrderHeader", "ShipmentScheduleImageID", "TMS.ImageGuid", "ID");
        }

        public override void Down()
        {
            DropForeignKey("TMS.OrderHeader", "ShipmentScheduleImageID", "TMS.ImageGuid");
            DropIndex("TMS.OrderHeader", new[] { "ShipmentScheduleImageID" });
            DropColumn("TMS.OrderHeader", "ShipmentScheduleImageID");
            DropColumn("TMS.OrderHeader", "Harga");
        }
    }
}
