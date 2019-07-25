namespace DMS.DataGateway.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ShippingListNo : DbMigration
    {
        public override void Up()
        {
            AddColumn("DMS.ShipmentList", "ShippingListNo", c => c.String(maxLength: 20));
        }
        
        public override void Down()
        {
            DropColumn("DMS.ShipmentList", "ShippingListNo");
        }
    }
}
