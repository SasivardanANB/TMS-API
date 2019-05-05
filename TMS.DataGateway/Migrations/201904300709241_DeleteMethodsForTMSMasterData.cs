namespace TMS.DataGateway.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DeleteMethodsForTMSMasterData : DbMigration
    {
        public override void Up()
        {
            DropColumn("TMS.Pool", "IsActive");
            DropColumn("TMS.Vehicle", "IsActive");
        }
        
        public override void Down()
        {
            AddColumn("TMS.Vehicle", "IsActive", c => c.Boolean(nullable: false));
            AddColumn("TMS.Pool", "IsActive", c => c.Boolean(nullable: false));
        }
    }
}
