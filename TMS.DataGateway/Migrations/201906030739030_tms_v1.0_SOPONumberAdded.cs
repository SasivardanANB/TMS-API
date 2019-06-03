namespace TMS.DataGateway.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class tms_v10_SOPONumberAdded : DbMigration
    {
        public override void Up()
        {
            AddColumn("TMS.OrderHeader", "SOPONumber", c => c.String(maxLength: 50));
        }
        
        public override void Down()
        {
            DropColumn("TMS.OrderHeader", "SOPONumber");
        }
    }
}
