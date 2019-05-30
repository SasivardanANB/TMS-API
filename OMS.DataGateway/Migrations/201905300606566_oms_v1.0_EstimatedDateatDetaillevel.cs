namespace OMS.DataGateway.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class oms_v10_EstimatedDateatDetaillevel : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("OMS.PackingSheet", "OrderDetailID", "OMS.OrderDetail");
            DropIndex("OMS.OrderHeader", "OrderHeader_OrderNo");
            DropIndex("OMS.PackingSheet", new[] { "OrderDetailID" });
            AddColumn("OMS.OrderDetail", "EstimationShipmentDate", c => c.DateTime(nullable: false));
            AddColumn("OMS.OrderDetail", "ActualShipmentDate", c => c.DateTime(nullable: false));
            AddColumn("OMS.PackingSheet", "ShippingListNo", c => c.String());
            AlterColumn("OMS.OrderHeader", "OrderNo", c => c.String(nullable: false, maxLength: 15));
            CreateIndex("OMS.OrderHeader", "OrderNo", unique: true, name: "OrderHeader_OrderNo");
            DropColumn("OMS.OrderHeader", "EstimationShipmentDate");
            DropColumn("OMS.OrderHeader", "ActualShipmentDate");
            DropColumn("OMS.PackingSheet", "OrderDetailID");
        }
        
        public override void Down()
        {
            AddColumn("OMS.PackingSheet", "OrderDetailID", c => c.Int(nullable: false));
            AddColumn("OMS.OrderHeader", "ActualShipmentDate", c => c.DateTime(nullable: false));
            AddColumn("OMS.OrderHeader", "EstimationShipmentDate", c => c.DateTime(nullable: false));
            DropIndex("OMS.OrderHeader", "OrderHeader_OrderNo");
            AlterColumn("OMS.OrderHeader", "OrderNo", c => c.String(nullable: false, maxLength: 20));
            DropColumn("OMS.PackingSheet", "ShippingListNo");
            DropColumn("OMS.OrderDetail", "ActualShipmentDate");
            DropColumn("OMS.OrderDetail", "EstimationShipmentDate");
            CreateIndex("OMS.PackingSheet", "OrderDetailID");
            CreateIndex("OMS.OrderHeader", "OrderNo", unique: true, name: "OrderHeader_OrderNo");
            AddForeignKey("OMS.PackingSheet", "OrderDetailID", "OMS.OrderDetail", "ID", cascadeDelete: true);
        }
    }
}
