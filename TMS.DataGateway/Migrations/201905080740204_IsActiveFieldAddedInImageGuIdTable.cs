namespace TMS.DataGateway.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class IsActiveFieldAddedInImageGuIdTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("TMS.ImageGuid", "IsActive", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("TMS.ImageGuid", "IsActive");
        }
    }
}
