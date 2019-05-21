namespace TMS.DataGateway.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Gate2GateRelatedTables : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "TMS.G2G",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        BusinessAreaId = c.Int(nullable: false),
                        G2GName = c.String(),
                        GateTypeId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("TMS.BusinessArea", t => t.BusinessAreaId, cascadeDelete: true)
                .ForeignKey("TMS.GateType", t => t.GateTypeId, cascadeDelete: false)
                .Index(t => t.BusinessAreaId)
                .Index(t => t.GateTypeId);
            
            CreateTable(
                "TMS.GateType",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        GateTypeDescription = c.String(maxLength: 10),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "TMS.GateInGateOut",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        OrderId = c.Int(nullable: false),
                        G2GId = c.Int(nullable: false),
                        GateTypeId = c.Int(nullable: false),
                        Info = c.String(),
                        CreatedBy = c.String(defaultValue:"SYSTEM"),
                        CreatedTime = c.DateTime(nullable: false,defaultValueSql:"GETDATE()"),
                        LastModifiedBy = c.String(),
                        LastModifiedTime = c.DateTime(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("TMS.G2G", t => t.G2GId, cascadeDelete: true)
                .ForeignKey("TMS.GateType", t => t.GateTypeId, cascadeDelete: true)
                .Index(t => t.G2GId)
                .Index(t => t.GateTypeId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("TMS.GateInGateOut", "GateTypeId", "TMS.GateType");
            DropForeignKey("TMS.GateInGateOut", "G2GId", "TMS.G2G");
            DropForeignKey("TMS.G2G", "GateTypeId", "TMS.GateType");
            DropForeignKey("TMS.G2G", "BusinessAreaId", "TMS.BusinessArea");
            DropIndex("TMS.GateInGateOut", new[] { "GateTypeId" });
            DropIndex("TMS.GateInGateOut", new[] { "G2GId" });
            DropIndex("TMS.G2G", new[] { "GateTypeId" });
            DropIndex("TMS.G2G", new[] { "BusinessAreaId" });
            DropTable("TMS.GateInGateOut");
            DropTable("TMS.GateType");
            DropTable("TMS.G2G");
        }
    }
}
