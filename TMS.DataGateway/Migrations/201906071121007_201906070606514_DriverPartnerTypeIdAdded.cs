namespace TMS.DataGateway.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _201906070606514_DriverPartnerTypeIdAdded : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("TMS.Driver", "TransporterId", "TMS.Partner");
            DropIndex("TMS.Driver", new[] { "TransporterId" });
            AlterColumn("TMS.Driver", "TransporterId", c => c.Int());
            CreateIndex("TMS.Driver", "TransporterId");
            AddForeignKey("TMS.Driver", "TransporterId", "TMS.Partner", "ID");
        }
        
        public override void Down()
        {
            DropForeignKey("TMS.Driver", "TransporterId", "TMS.Partner");
            DropIndex("TMS.Driver", new[] { "TransporterId" });
            AlterColumn("TMS.Driver", "TransporterId", c => c.Int(nullable: false));
            CreateIndex("TMS.Driver", "TransporterId");
            AddForeignKey("TMS.Driver", "TransporterId", "TMS.Partner", "ID", cascadeDelete: true);
        }
    }
}
