namespace OMS.DataGateway.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class oms_v10_Business : DbMigration
    {
        public override void Up()
        {
            DropIndex("OMS.BusinessArea", "BusinessArea_CompanyCodeID");
            CreateIndex("OMS.BusinessArea", "CompanyCodeID");
        }
        
        public override void Down()
        {
            DropIndex("OMS.BusinessArea", new[] { "CompanyCodeID" });
            CreateIndex("OMS.BusinessArea", "CompanyCodeID", unique: true, name: "BusinessArea_CompanyCodeID");
        }
    }
}
