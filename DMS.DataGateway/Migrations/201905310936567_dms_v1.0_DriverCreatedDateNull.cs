namespace DMS.DataGateway.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class dms_v10_DriverCreatedDateNull : DbMigration
    {
        public override void Up()
        {
            AlterColumn("DMS.Driver", "CreatedTime", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("DMS.Driver", "CreatedTime", c => c.DateTime(nullable: false));
        }
    }
}
