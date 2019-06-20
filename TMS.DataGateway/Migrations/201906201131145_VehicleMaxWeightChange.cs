namespace TMS.DataGateway.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class VehicleMaxWeightChange : DbMigration
    {
        public override void Up()
        {
            AlterColumn("TMS.Vehicle", "MaxWeight", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            AlterColumn("TMS.Vehicle", "MaxWeight", c => c.Int(nullable: false));
        }
    }
}
