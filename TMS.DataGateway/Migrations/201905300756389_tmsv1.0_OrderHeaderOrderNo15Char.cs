namespace TMS.DataGateway.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class tmsv10_OrderHeaderOrderNo15Char : DbMigration
    {
        public override void Up()
        {
            DropIndex("TMS.OrderHeader", "OrderHeader_OrderNo");
            AlterColumn("TMS.OrderHeader", "OrderNo", c => c.String(nullable: false, maxLength: 15));
            CreateIndex("TMS.OrderHeader", "OrderNo", unique: true, name: "OrderHeader_OrderNo");
        }
        
        public override void Down()
        {
            DropIndex("TMS.OrderHeader", "OrderHeader_OrderNo");
            AlterColumn("TMS.OrderHeader", "OrderNo", c => c.String(nullable: false, maxLength: 20));
            CreateIndex("TMS.OrderHeader", "OrderNo", unique: true, name: "OrderHeader_OrderNo");
        }
    }
}
