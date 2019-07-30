namespace TMS.DataGateway.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PoolPhotoId : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("TMS.Pool", "PhotoId", "TMS.ImageGuid");
            DropIndex("TMS.Pool", new[] { "PhotoId" });
            AlterColumn("TMS.Pool", "PhotoId", c => c.Int());
            CreateIndex("TMS.Pool", "PhotoId");
            AddForeignKey("TMS.Pool", "PhotoId", "TMS.ImageGuid", "ID");
        }
        
        public override void Down()
        {
            DropForeignKey("TMS.Pool", "PhotoId", "TMS.ImageGuid");
            DropIndex("TMS.Pool", new[] { "PhotoId" });
            AlterColumn("TMS.Pool", "PhotoId", c => c.Int(nullable: false));
            CreateIndex("TMS.Pool", "PhotoId");
            AddForeignKey("TMS.Pool", "PhotoId", "TMS.ImageGuid", "ID", cascadeDelete: true);
        }
    }
}
