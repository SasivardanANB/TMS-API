namespace TMS.DataGateway.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ImageGuIdColumns_Add : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "TMS.ImageGuid",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        ImageGuIdValue = c.String(maxLength: 1000),
                        CreatedBy = c.String(defaultValue: "SYSTEM"),
                        CreatedTime = c.DateTime(nullable: false, defaultValueSql: "GETDATE()"),
                        LastModifiedBy = c.String(),
                        LastModifiedTime = c.DateTime(),
                    })
                .PrimaryKey(t => t.ID);
            
            AddColumn("TMS.Driver", "IdentityImageGuId", c => c.Int(nullable: false));
            AddColumn("TMS.Driver", "DrivingLicenceImageGuId", c => c.Int(nullable: false));
            AddColumn("TMS.Driver", "DriverImageGuId", c => c.Int(nullable: false));
            AddColumn("TMS.PIC", "PhotoGuId", c => c.Int(nullable: false));
            AddColumn("TMS.Pool", "PhotoGuId", c => c.Int(nullable: false));
            CreateIndex("TMS.Driver", "IdentityImageGuId");
            CreateIndex("TMS.PIC", "PhotoGuId");
            CreateIndex("TMS.Pool", "PhotoGuId");
            AddForeignKey("TMS.Driver", "IdentityImageGuId", "TMS.ImageGuid", "ID", cascadeDelete: true);
            AddForeignKey("TMS.PIC", "PhotoGuId", "TMS.ImageGuid", "ID", cascadeDelete: true);
            AddForeignKey("TMS.Pool", "PhotoGuId", "TMS.ImageGuid", "ID", cascadeDelete: true);
            DropColumn("TMS.Driver", "IdentityImage");
            DropColumn("TMS.Driver", "DrivingLicenceImage");
            DropColumn("TMS.Driver", "DriverImage");
            DropColumn("TMS.Pool", "Photo");
        }
        
        public override void Down()
        {
            AddColumn("TMS.Pool", "Photo", c => c.String(maxLength: 200));
            AddColumn("TMS.Driver", "DriverImage", c => c.String(maxLength: 200));
            AddColumn("TMS.Driver", "DrivingLicenceImage", c => c.String(maxLength: 200));
            AddColumn("TMS.Driver", "IdentityImage", c => c.String(maxLength: 200));
            DropForeignKey("TMS.Pool", "PhotoGuId", "TMS.ImageGuid");
            DropForeignKey("TMS.PIC", "PhotoGuId", "TMS.ImageGuid");
            DropForeignKey("TMS.Driver", "IdentityImageGuId", "TMS.ImageGuid");
            DropIndex("TMS.Pool", new[] { "PhotoGuId" });
            DropIndex("TMS.PIC", new[] { "PhotoGuId" });
            DropIndex("TMS.Driver", new[] { "IdentityImageGuId" });
            DropColumn("TMS.Pool", "PhotoGuId");
            DropColumn("TMS.PIC", "PhotoGuId");
            DropColumn("TMS.Driver", "DriverImageGuId");
            DropColumn("TMS.Driver", "DrivingLicenceImageGuId");
            DropColumn("TMS.Driver", "IdentityImageGuId");
            DropTable("TMS.ImageGuid");
        }
    }
}
