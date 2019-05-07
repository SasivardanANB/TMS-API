namespace TMS.DataGateway.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class tms_v10_Partner_ShipmentSAP_PackingSheet : DbMigration
    {
        public override void Up()
        {
            AlterColumn("TMS.PackingSheet", "CreatedTime", c => c.DateTime(nullable: false));
            AlterColumn("TMS.ShipmentSAP", "CreatedTime", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("TMS.ShipmentSAP", "CreatedTime", c => c.DateTime(nullable: false));
            AlterColumn("TMS.PackingSheet", "CreatedTime", c => c.DateTime(nullable: false));
        }
    }
}
