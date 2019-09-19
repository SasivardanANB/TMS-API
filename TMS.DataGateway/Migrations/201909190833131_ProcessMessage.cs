namespace TMS.DataGateway.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ProcessMessage : DbMigration
    {
        public override void Up()
        {
            AddColumn("TMS.ShipmentScheduleOCRDetails", "ProcessMessage", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("TMS.ShipmentScheduleOCRDetails", "ProcessMessage");
        }
    }
}
