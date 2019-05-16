namespace DMS.DataGateway.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TripStatusColumnAdded : DbMigration
    {
        public override void Up()
        {
            AddColumn("DMS.TripManager", "CurrentTripStatusId", c => c.Int());
            CreateIndex("DMS.TripManager", "CurrentTripStatusId");
            AddForeignKey("DMS.TripManager", "CurrentTripStatusId", "DMS.TripStatus", "ID");
        }
        
        public override void Down()
        {
            DropForeignKey("DMS.TripManager", "CurrentTripStatusId", "DMS.TripStatus");
            DropIndex("DMS.TripManager", new[] { "CurrentTripStatusId" });
            DropColumn("DMS.TripManager", "CurrentTripStatusId");
        }
    }
}
