namespace DMS.DataGateway.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class dms_v10_TripDate : DbMigration
    {
        public override void Up()
        {
            AddColumn("DMS.TripManager", "TripDate", c => c.DateTime(nullable: false));
            AddColumn("DMS.TripManager", "BusinessAreaCode", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("DMS.TripManager", "BusinessAreaCode");
            DropColumn("DMS.TripManager", "TripDate");
        }
    }
}
