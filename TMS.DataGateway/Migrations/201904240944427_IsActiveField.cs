namespace TMS.DataGateway.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class IsActiveField : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "TMS.Pool",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        PoolNo = c.String(maxLength: 15),
                        PoolCodeName = c.String(maxLength: 10),
                        PoolName = c.String(maxLength: 50),
                        Address = c.String(maxLength: 255),
                        ContactNumber = c.String(maxLength: 15),
                        CityID = c.Int(nullable: false),
                        Photo = c.String(maxLength: 200),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("TMS.City", t => t.CityID, cascadeDelete: true)
                .Index(t => t.CityID);
            
            CreateTable(
                "TMS.Vehicle",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        PlateNumber = c.String(nullable: false, maxLength: 12),
                        VehicleTypeID = c.Int(nullable: false),
                        VehicleTypeName = c.String(maxLength: 20),
                        PoliceNo = c.String(maxLength: 12),
                        MaxWeight = c.Int(nullable: false),
                        MaxDimension = c.String(maxLength: 12),
                        KIRNo = c.String(maxLength: 25),
                        KIRExpiryDate = c.DateTime(nullable: false),
                        PoolID = c.Int(nullable: false),
                        IsDedicated = c.Boolean(nullable: false),
                        ShipperID = c.Int(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("TMS.Expeditor", t => t.ShipperID, cascadeDelete: true)
                .ForeignKey("TMS.Pool", t => t.PoolID, cascadeDelete: false)
                .ForeignKey("TMS.VehicleType", t => t.VehicleTypeID, cascadeDelete: true)
                .Index(t => t.VehicleTypeID)
                .Index(t => t.PoolID)
                .Index(t => t.ShipperID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("TMS.Vehicle", "VehicleTypeID", "TMS.VehicleType");
            DropForeignKey("TMS.Vehicle", "PoolID", "TMS.Pool");
            DropForeignKey("TMS.Vehicle", "ShipperID", "TMS.Expeditor");
            DropForeignKey("TMS.Pool", "CityID", "TMS.City");
            DropIndex("TMS.Vehicle", new[] { "ShipperID" });
            DropIndex("TMS.Vehicle", new[] { "PoolID" });
            DropIndex("TMS.Vehicle", new[] { "VehicleTypeID" });
            DropIndex("TMS.Pool", new[] { "CityID" });
            DropTable("TMS.Vehicle");
            DropTable("TMS.Pool");
        }
    }
}
