namespace TMS.DataGateway.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Pendingchanges_v2 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "TMS.FleetType",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        FleetTypeDescription = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
            AddColumn("TMS.OrderHeader", "FleetTypeID", c => c.Int(nullable: false));
            CreateIndex("TMS.OrderHeader", "FleetTypeID");
            AddForeignKey("TMS.OrderHeader", "FleetTypeID", "TMS.FleetType", "ID", cascadeDelete: true);
            DropColumn("TMS.OrderHeader", "FleetType");
        }
        
        public override void Down()
        {
            AddColumn("TMS.OrderHeader", "FleetType", c => c.Int(nullable: false));
            DropForeignKey("TMS.OrderHeader", "FleetTypeID", "TMS.FleetType");
            DropIndex("TMS.OrderHeader", new[] { "FleetTypeID" });
            DropColumn("TMS.OrderHeader", "FleetTypeID");
            DropTable("TMS.FleetType");
        }
    }
}
