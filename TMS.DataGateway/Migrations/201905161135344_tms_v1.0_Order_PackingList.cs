namespace TMS.DataGateway.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class tms_v10_Order_PackingList : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("TMS.PackingSheet", "OrderDetailID", "TMS.OrderDetail");
            DropIndex("TMS.PackingSheet", new[] { "OrderDetailID" });
        }
        
        public override void Down()
        {
            AddColumn("TMS.PackingSheet", "OrderDetailID", c => c.Int(nullable: false));
            AddForeignKey("TMS.PackingSheet", "OrderDetailID", "TMS.OrderDetail", "ID", cascadeDelete: true);
        }
    }
}
