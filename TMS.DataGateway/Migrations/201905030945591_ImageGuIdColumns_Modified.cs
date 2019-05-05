namespace TMS.DataGateway.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ImageGuIdColumns_Modified : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "TMS.Driver", name: "IdentityImageGuId", newName: "IdentityImageId");
            RenameColumn(table: "TMS.PIC", name: "PhotoGuId", newName: "PhotoId");
            RenameColumn(table: "TMS.Pool", name: "PhotoGuId", newName: "PhotoId");
            RenameIndex(table: "TMS.Driver", name: "IX_IdentityImageGuId", newName: "IX_IdentityImageId");
            RenameIndex(table: "TMS.PIC", name: "IX_PhotoGuId", newName: "IX_PhotoId");
            RenameIndex(table: "TMS.Pool", name: "IX_PhotoGuId", newName: "IX_PhotoId");
            AddColumn("TMS.Driver", "DrivingLicenceImageId", c => c.Int(nullable: false));
            AddColumn("TMS.Driver", "DriverImageId", c => c.Int(nullable: false));
            DropColumn("TMS.Driver", "DrivingLicenceImageGuId");
            DropColumn("TMS.Driver", "DriverImageGuId");
        }
        
        public override void Down()
        {
            AddColumn("TMS.Driver", "DriverImageGuId", c => c.Int(nullable: false));
            AddColumn("TMS.Driver", "DrivingLicenceImageGuId", c => c.Int(nullable: false));
            DropColumn("TMS.Driver", "DriverImageId");
            DropColumn("TMS.Driver", "DrivingLicenceImageId");
            RenameIndex(table: "TMS.Pool", name: "IX_PhotoId", newName: "IX_PhotoGuId");
            RenameIndex(table: "TMS.PIC", name: "IX_PhotoId", newName: "IX_PhotoGuId");
            RenameIndex(table: "TMS.Driver", name: "IX_IdentityImageId", newName: "IX_IdentityImageGuId");
            RenameColumn(table: "TMS.Pool", name: "PhotoId", newName: "PhotoGuId");
            RenameColumn(table: "TMS.PIC", name: "PhotoId", newName: "PhotoGuId");
            RenameColumn(table: "TMS.Driver", name: "IdentityImageId", newName: "IdentityImageGuId");
        }
    }
}
