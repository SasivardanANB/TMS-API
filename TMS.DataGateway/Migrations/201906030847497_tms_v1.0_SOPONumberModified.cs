namespace TMS.DataGateway.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class tms_v10_SOPONumberModified : DbMigration
    {
        public override void Up()
        {
            AlterColumn("TMS.OrderHeader", "SOPONumber", c => c.String(maxLength: 10));
        }
        
        public override void Down()
        {
            AlterColumn("TMS.OrderHeader", "SOPONumber", c => c.String(maxLength: 50));
        }
    }
}
