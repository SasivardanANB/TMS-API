namespace TMS.DataGateway.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PICChanges : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("TMS.PIC");
            AddColumn("TMS.PIC", "IsDeleted", c => c.Boolean(nullable: false));
            AlterColumn("TMS.PIC", "ID", c => c.Int(nullable: false, identity: true));
            AddPrimaryKey("TMS.PIC", "ID");
            CreateIndex("TMS.Partner", "PostalCodeID");
            CreateIndex("TMS.Partner", "PICID");
            AddForeignKey("TMS.Partner", "PICID", "TMS.PIC", "ID", cascadeDelete: true);
            AddForeignKey("TMS.Partner", "PostalCodeID", "TMS.PostalCode", "ID", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("TMS.Partner", "PostalCodeID", "TMS.PostalCode");
            DropForeignKey("TMS.Partner", "PICID", "TMS.PIC");
            DropIndex("TMS.Partner", new[] { "PICID" });
            DropIndex("TMS.Partner", new[] { "PostalCodeID" });
            DropPrimaryKey("TMS.PIC");
            AlterColumn("TMS.PIC", "ID", c => c.Long(nullable: false, identity: true));
            DropColumn("TMS.PIC", "IsDeleted");
            AddPrimaryKey("TMS.PIC", "ID");
        }
    }
}
