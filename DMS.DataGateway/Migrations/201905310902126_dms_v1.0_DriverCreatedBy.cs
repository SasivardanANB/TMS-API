namespace DMS.DataGateway.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class dms_v10_DriverCreatedBy : DbMigration
    {
        public override void Up()
        {
            AddColumn("DMS.Driver", "CreatedBy", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("DMS.Driver", "CreatedBy");
        }
    }
}
