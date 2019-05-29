namespace OMS.DataGateway.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class oms_v10_Issues : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("OMS.Partner", "PartnerTypeID", "OMS.PartnerType");
            DropIndex("OMS.Partner", new[] { "PartnerTypeID" });
            DropColumn("OMS.Partner", "PartnerTypeID");
        }
        
        public override void Down()
        {
            AddColumn("OMS.Partner", "PartnerTypeID", c => c.Int(nullable: false));
            CreateIndex("OMS.Partner", "PartnerTypeID");
            AddForeignKey("OMS.Partner", "PartnerTypeID", "OMS.PartnerType", "ID", cascadeDelete: true);
        }
    }
}
