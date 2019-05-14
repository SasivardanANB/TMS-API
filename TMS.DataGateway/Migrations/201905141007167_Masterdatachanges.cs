namespace TMS.DataGateway.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Masterdatachanges : DbMigration
    {
        public override void Up()
        {
            DropIndex("TMS.Pool", "Pool_PoolCode");
            AddColumn("TMS.Driver", "UserName", c => c.String(nullable: false, maxLength: 15));
            AddColumn("TMS.Pool", "PoolDescritpion", c => c.String(maxLength: 50));
            AlterColumn("TMS.Driver", "DrivingLicenseExpiredDate", c => c.DateTime());
            AlterColumn("TMS.Pool", "PoolName", c => c.String(maxLength: 10));
            AlterColumn("TMS.Vehicle", "KIRExpiryDate", c => c.DateTime());
            CreateIndex("TMS.Pool", "PoolName", unique: true, name: "Pool_PoolName");
            DropColumn("TMS.Pool", "PoolCode");
            DropColumn("TMS.Vehicle", "VehicleTypeName");
        }
        
        public override void Down()
        {
            AddColumn("TMS.Vehicle", "VehicleTypeName", c => c.String(maxLength: 20));
            AddColumn("TMS.Pool", "PoolCode", c => c.String(maxLength: 10));
            DropIndex("TMS.Pool", "Pool_PoolName");
            AlterColumn("TMS.Vehicle", "KIRExpiryDate", c => c.DateTime(nullable: false));
            AlterColumn("TMS.Pool", "PoolName", c => c.String(maxLength: 50));
            AlterColumn("TMS.Driver", "DrivingLicenseExpiredDate", c => c.DateTime(nullable: false));
            DropColumn("TMS.Pool", "PoolDescritpion");
            DropColumn("TMS.Driver", "UserName");
            CreateIndex("TMS.Pool", "PoolCode", unique: true, name: "Pool_PoolCode");
        }
    }
}
