namespace TMS.DataGateway.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class tms_v10_PartnerType : DbMigration
    {
        public override void Up()
        {
            AddColumn("TMS.PartnerType", "CreatedBy", c => c.String(defaultValue: "SYSTEM"));
            AddColumn("TMS.PartnerType", "CreatedTime", c => c.DateTime(nullable: false, defaultValueSql: "GETDATE()"));
            AddColumn("TMS.PartnerType", "LastModifiedBy", c => c.String());
            AddColumn("TMS.PartnerType", "LastModifiedTime", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("TMS.PartnerType", "LastModifiedTime");
            DropColumn("TMS.PartnerType", "LastModifiedBy");
            DropColumn("TMS.PartnerType", "CreatedTime");
            DropColumn("TMS.PartnerType", "CreatedBy");
        }
    }
}
