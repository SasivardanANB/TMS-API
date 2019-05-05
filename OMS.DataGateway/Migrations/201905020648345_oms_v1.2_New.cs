namespace OMS.DataGateway.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class oms_v12_New : DbMigration
    {
        public override void Up()
        {
            AddColumn("OMS.OrderHeader", "OrderStatusID", c => c.Int(nullable: false));
            DropColumn("OMS.OrderHeader", "OrderShipmentStatus");
        }
        
        public override void Down()
        {
            AddColumn("OMS.OrderHeader", "OrderShipmentStatus", c => c.Int(nullable: false));
            DropColumn("OMS.OrderHeader", "OrderStatusID");
        }
    }
}
