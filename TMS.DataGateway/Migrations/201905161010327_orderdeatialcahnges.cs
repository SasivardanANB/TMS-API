namespace TMS.DataGateway.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class orderdeatialcahnges : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("TMS.PackingSheet", "OrderDetailID", "TMS.OrderDetail");
            DropIndex("TMS.PackingSheet", new[] { "OrderDetailID" });
            AddColumn("TMS.OrderDetail", "Katerangan", c => c.String());
            AddColumn("TMS.PackingSheet", "ShippingListNo", c => c.String());
            DropColumn("TMS.PackingSheet", "OrderDetailID");
        }
        
        public override void Down()
        {
            AddColumn("TMS.PackingSheet", "OrderDetailID", c => c.Int(nullable: false));
            DropColumn("TMS.PackingSheet", "ShippingListNo");
            DropColumn("TMS.OrderDetail", "Katerangan");
            CreateIndex("TMS.PackingSheet", "OrderDetailID");
            AddForeignKey("TMS.PackingSheet", "OrderDetailID", "TMS.OrderDetail", "ID", cascadeDelete: true);
        }
    }
}
