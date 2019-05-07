namespace TMS.DataGateway.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class latestmigration : DbMigration
    {
        public override void Up()
        {
            AlterColumn("TMS.Driver", "DriverName", c => c.String(nullable: false, maxLength: 30));
            AlterColumn("TMS.Driver", "DriverPhone", c => c.String(nullable: false, maxLength: 15));
            AlterColumn("TMS.Driver", "Password", c => c.String(nullable: false, maxLength: 20));
        }
        
        public override void Down()
        {
            AlterColumn("TMS.Driver", "Password", c => c.String(maxLength: 20));
            AlterColumn("TMS.Driver", "DriverPhone", c => c.String(maxLength: 15));
            AlterColumn("TMS.Driver", "DriverName", c => c.String(maxLength: 30));
        }
    }
}
