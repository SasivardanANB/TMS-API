namespace OMS.DataGateway.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class omsorderheaderchange : DbMigration
    {
        public override void Up()
        {
            AddColumn("OMS.ImageGuid", "CreatedBy", c => c.String(maxLength: 100));
            AddColumn("OMS.ImageGuid", "CreatedTime", c => c.DateTime(nullable: false));
            AddColumn("OMS.ImageGuid", "LastModifiedBy", c => c.String(maxLength: 100));
            AddColumn("OMS.ImageGuid", "LastModifiedTime", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("OMS.ImageGuid", "LastModifiedTime");
            DropColumn("OMS.ImageGuid", "LastModifiedBy");
            DropColumn("OMS.ImageGuid", "CreatedTime");
            DropColumn("OMS.ImageGuid", "CreatedBy");
        }
    }
}
