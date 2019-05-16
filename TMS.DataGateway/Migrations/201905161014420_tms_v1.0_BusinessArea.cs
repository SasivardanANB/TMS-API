namespace TMS.DataGateway.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class tms_v10_BusinessArea : DbMigration
    {
        public override void Up()
        {
            DropIndex("TMS.BusinessArea", "BusinessArea_CompanyCodeID");
            CreateIndex("TMS.BusinessArea", "CompanyCodeID");
        }
        
        public override void Down()
        {
            DropIndex("TMS.BusinessArea", new[] { "CompanyCodeID" });
            CreateIndex("TMS.BusinessArea", "CompanyCodeID", unique: true, name: "BusinessArea_CompanyCodeID");
        }
    }
}
