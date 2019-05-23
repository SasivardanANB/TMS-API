namespace TMS.DataGateway.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class partnerchanges : DbMigration
    {
        public override void Up()
        {
            DropIndex("TMS.Partner", "Partner_PartnerNo");
        }
        
        public override void Down()
        {
            CreateIndex("TMS.Partner", "PartnerNo", unique: true, name: "Partner_PartnerNo");
        }
    }
}
