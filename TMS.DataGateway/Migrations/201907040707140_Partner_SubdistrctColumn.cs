namespace TMS.DataGateway.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Partner_SubdistrctColumn : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("TMS.Partner", "PostalCodeID", "TMS.PostalCode");
            DropIndex("TMS.Partner", new[] { "PostalCodeID" });
            AddColumn("TMS.Partner", "SubDistrictID", c => c.Int());
            CreateIndex("TMS.Partner", "SubDistrictID");
            AddForeignKey("TMS.Partner", "SubDistrictID", "TMS.SubDistrict", "ID");
            DropColumn("TMS.Partner", "PostalCodeID");
        }
        
        public override void Down()
        {
            AddColumn("TMS.Partner", "PostalCodeID", c => c.Int());
            DropForeignKey("TMS.Partner", "SubDistrictID", "TMS.SubDistrict");
            DropIndex("TMS.Partner", new[] { "SubDistrictID" });
            DropColumn("TMS.Partner", "SubDistrictID");
            CreateIndex("TMS.Partner", "PostalCodeID");
            AddForeignKey("TMS.Partner", "PostalCodeID", "TMS.PostalCode", "ID");
        }
    }
}
