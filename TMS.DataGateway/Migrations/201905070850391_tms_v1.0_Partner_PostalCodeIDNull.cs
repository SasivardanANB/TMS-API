namespace TMS.DataGateway.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class tms_v10_Partner_PostalCodeIDNull : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("TMS.Partner", "PostalCodeID", "TMS.PostalCode");
            DropIndex("TMS.Partner", new[] { "PostalCodeID" });
            AlterColumn("TMS.Partner", "PostalCodeID", c => c.Int());
            CreateIndex("TMS.Partner", "PostalCodeID");
            AddForeignKey("TMS.Partner", "PostalCodeID", "TMS.PostalCode", "ID");
        }
        
        public override void Down()
        {
            DropForeignKey("TMS.Partner", "PostalCodeID", "TMS.PostalCode");
            DropIndex("TMS.Partner", new[] { "PostalCodeID" });
            AlterColumn("TMS.Partner", "PostalCodeID", c => c.Int(nullable: false));
            CreateIndex("TMS.Partner", "PostalCodeID");
            AddForeignKey("TMS.Partner", "PostalCodeID", "TMS.PostalCode", "ID", cascadeDelete: true);
        }
    }
}
