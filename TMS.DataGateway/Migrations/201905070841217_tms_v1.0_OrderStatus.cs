namespace TMS.DataGateway.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class tms_v10_OrderStatus : DbMigration
    {
        public override void Up()
        {
            AlterColumn("TMS.OrderStatus", "CreatedTime", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("TMS.OrderStatus", "CreatedTime", c => c.DateTime(nullable: false));
        }
    }
}
