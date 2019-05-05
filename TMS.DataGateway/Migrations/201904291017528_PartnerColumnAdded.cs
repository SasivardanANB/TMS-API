namespace TMS.DataGateway.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PartnerColumnAdded : DbMigration
    {
        public override void Up()
        {
            DropIndex("TMS.Partner", "Partner_OrderPointCode");
            CreateTable(
                "TMS.PIC",
                c => new
                    {
                        ID = c.Long(nullable: false, identity: true),
                        PICName = c.String(maxLength: 50),
                        PICPhone = c.String(maxLength: 15),
                        PICEmail = c.String(maxLength: 50),
                        IsActive = c.Boolean(nullable: false),
                        PICPassword = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => t.ID);
            
            AddColumn("TMS.Driver", "FirstName", c => c.String(nullable: false, maxLength: 15));
            AddColumn("TMS.Driver", "LastName", c => c.String(nullable: false, maxLength: 15));
            AddColumn("TMS.Driver", "IsDelete", c => c.Boolean(nullable: false));
            AddColumn("TMS.Partner", "PartnerAddress", c => c.String());
            AddColumn("TMS.Partner", "PICID", c => c.Int(nullable: false));
            AddColumn("TMS.Partner", "IsDeleted", c => c.Boolean(nullable: false));
            AddColumn("TMS.Pool", "PoolCode", c => c.String(maxLength: 10));
            AddColumn("TMS.Pool", "IsDelete", c => c.Boolean(nullable: false));
            AddColumn("TMS.Vehicle", "IsDelete", c => c.Boolean(nullable: false));
            AlterColumn("TMS.Partner", "OrderPointTypeID", c => c.Int());
            CreateIndex("TMS.Pool", "PoolNo", unique: true, name: "Pool_PoolNo");
            CreateIndex("TMS.Pool", "PoolCode", unique: true, name: "Pool_PoolCode");
            DropColumn("TMS.Driver", "DriverName");
            DropColumn("TMS.Pool", "PoolCodeName");
        }
        
        public override void Down()
        {
            AddColumn("TMS.Pool", "PoolCodeName", c => c.String(maxLength: 10));
            AddColumn("TMS.Driver", "DriverName", c => c.String(nullable: false, maxLength: 30));
            DropIndex("TMS.Pool", "Pool_PoolCode");
            DropIndex("TMS.Pool", "Pool_PoolNo");
            AlterColumn("TMS.Partner", "OrderPointTypeID", c => c.Int(nullable: false));
            DropColumn("TMS.Vehicle", "IsDelete");
            DropColumn("TMS.Pool", "IsDelete");
            DropColumn("TMS.Pool", "PoolCode");
            DropColumn("TMS.Partner", "IsDeleted");
            DropColumn("TMS.Partner", "PICID");
            DropColumn("TMS.Partner", "PartnerAddress");
            DropColumn("TMS.Driver", "IsDelete");
            DropColumn("TMS.Driver", "LastName");
            DropColumn("TMS.Driver", "FirstName");
            DropTable("TMS.PIC");
            CreateIndex("TMS.Partner", "OrderPointCode", unique: true, name: "Partner_OrderPointCode");
        }
    }
}
