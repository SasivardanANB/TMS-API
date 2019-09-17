namespace DMS.DataGateway.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PartnerNo : DbMigration
    {
        public override void Up()
        {
            DropIndex("DMS.Partner", "Partner_PartnerNo");
            AlterColumn("DMS.Partner", "PartnerNo", c => c.String(maxLength: 10));
            CreateIndex("DMS.Partner", "PartnerNo", unique: true, name: "Partner_PartnerNo");
        }
        
        public override void Down()
        {
            DropIndex("DMS.Partner", "Partner_PartnerNo");
            AlterColumn("DMS.Partner", "PartnerNo", c => c.String(maxLength: 10));
            CreateIndex("DMS.Partner", "PartnerNo", unique: true, name: "Partner_PartnerNo");
        }
    }
}
