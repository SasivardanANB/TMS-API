namespace TMS.DataGateway.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DriverPartnerTypeIdAdded : DbMigration
    {
        public override void Up()
        {
            AddColumn("TMS.Driver", "TransporterId", c => c.Int(nullable: false));
            CreateIndex("TMS.Driver", "TransporterId");
            AddForeignKey("TMS.Driver", "TransporterId", "TMS.Partner", "ID", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("TMS.Driver", "TransporterId", "TMS.Partner");
            DropIndex("TMS.Driver", new[] { "TransporterId" });
            DropColumn("TMS.Driver", "TransporterId");
        }
    }
}
