namespace TMS.DataGateway.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UploadTypeAddedToOrederhTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("TMS.OrderHeader", "UploadType", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("TMS.OrderHeader", "UploadType");
        }
    }
}
