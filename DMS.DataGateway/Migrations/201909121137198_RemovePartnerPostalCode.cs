namespace DMS.DataGateway.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemovePartnerPostalCode : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("DMS.Partner", "PostalCodeId", "DMS.PostalCode");
            DropIndex("DMS.Partner", new[] { "PostalCodeId" });
            DropColumn("DMS.Partner", "PostalCodeId");
        }
        
        public override void Down()
        {
            AddColumn("DMS.Partner", "PostalCodeId", c => c.Int(nullable: false));
            CreateIndex("DMS.Partner", "PostalCodeId");
            AddForeignKey("DMS.Partner", "PostalCodeId", "DMS.PostalCode", "ID", cascadeDelete: true);
        }
    }
}
