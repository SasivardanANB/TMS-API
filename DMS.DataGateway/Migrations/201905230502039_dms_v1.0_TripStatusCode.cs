namespace DMS.DataGateway.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class dms_v10_TripStatusCode : DbMigration
    {
        public override void Up()
        {
            AddColumn("DMS.TripStatus", "StatusCode", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("DMS.TripStatus", "StatusCode");
        }
    }
}
