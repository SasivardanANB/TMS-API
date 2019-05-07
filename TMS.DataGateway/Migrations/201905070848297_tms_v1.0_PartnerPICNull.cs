namespace TMS.DataGateway.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class tms_v10_PartnerPICNull : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("TMS.Partner", "PICID", "TMS.PIC");
            DropIndex("TMS.Partner", new[] { "PICID" });
            AlterColumn("TMS.Partner", "PICID", c => c.Int());
            CreateIndex("TMS.Partner", "PICID");
            AddForeignKey("TMS.Partner", "PICID", "TMS.PIC", "ID");
        }
        
        public override void Down()
        {
            DropForeignKey("TMS.Partner", "PICID", "TMS.PIC");
            DropIndex("TMS.Partner", new[] { "PICID" });
            AlterColumn("TMS.Partner", "PICID", c => c.Int(nullable: false));
            CreateIndex("TMS.Partner", "PICID");
            AddForeignKey("TMS.Partner", "PICID", "TMS.PIC", "ID", cascadeDelete: true);
        }
    }
}
