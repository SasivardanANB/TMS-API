namespace TMS.DataGateway.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class tms_v10_IsLoad : DbMigration
    {
        public override void Up()
        {
            AddColumn("TMS.OrderStatusHistory", "IsLoad", c => c.Boolean());
        }
        
        public override void Down()
        {
            DropColumn("TMS.OrderStatusHistory", "IsLoad");
        }
    }
}
