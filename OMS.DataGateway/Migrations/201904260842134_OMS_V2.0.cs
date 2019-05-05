namespace OMS.DataGateway.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class OMS_V20 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("OMS.User", "BusinessArea_ID", "OMS.BusinessArea");
            DropIndex("OMS.User", new[] { "BusinessArea_ID" });
            DropColumn("OMS.User", "BusinessArea_ID");
        }
        
        public override void Down()
        {
            AddColumn("OMS.User", "BusinessArea_ID", c => c.Int());
            CreateIndex("OMS.User", "BusinessArea_ID");
            AddForeignKey("OMS.User", "BusinessArea_ID", "OMS.BusinessArea", "ID");
        }
    }
}
