namespace TMS.DataGateway.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PendingChanges : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("TMS.Expeditor", "ExpeditorTypeID", "TMS.ExpeditorType");
            DropForeignKey("TMS.Expeditor", "PostalCodeID", "TMS.PostalCode");
            DropForeignKey("TMS.Vehicle", "ShipperID", "TMS.Expeditor");
            DropIndex("TMS.Expeditor", new[] { "PostalCodeID" });
            DropIndex("TMS.Expeditor", new[] { "ExpeditorTypeID" });
            AddColumn("TMS.Driver", "CreatedBy", c => c.String(defaultValue: "SYSTEM"));
            AddColumn("TMS.Driver", "CreatedTime", c => c.DateTime(nullable: false, defaultValueSql: "GETDATE()"));
            AddColumn("TMS.Driver", "LastModifiedBy", c => c.String());
            AddColumn("TMS.Driver", "LastModifiedTime", c => c.DateTime());
            AddColumn("TMS.PIC", "CreatedBy", c => c.String(defaultValue: "SYSTEM"));
            AddColumn("TMS.PIC", "CreatedTime", c => c.DateTime(nullable: false, defaultValueSql: "GETDATE()"));
            AddColumn("TMS.PIC", "LastModifiedBy", c => c.String());
            AddColumn("TMS.PIC", "LastModifiedTime", c => c.DateTime());
            AddColumn("TMS.Pool", "CreatedBy", c => c.String(defaultValue: "SYSTEM"));
            AddColumn("TMS.Pool", "CreatedTime", c => c.DateTime(nullable: false, defaultValueSql: "GETDATE()"));
            AddColumn("TMS.Pool", "LastModifiedBy", c => c.String());
            AddColumn("TMS.Pool", "LastModifiedTime", c => c.DateTime());
            AddColumn("TMS.Vehicle", "CreatedBy", c => c.String(defaultValue: "SYSTEM"));
            AddColumn("TMS.Vehicle", "CreatedTime", c => c.DateTime(nullable: false, defaultValueSql: "GETDATE()"));
            AddColumn("TMS.Vehicle", "LastModifiedBy", c => c.String());
            AddColumn("TMS.Vehicle", "LastModifiedTime", c => c.DateTime());
            AlterColumn("TMS.OrderStatus", "CreatedTime", c => c.DateTime(nullable: false, defaultValueSql: "GETDATE()"));
            AddForeignKey("TMS.Vehicle", "ShipperID", "TMS.Partner", "ID", cascadeDelete: true);
            DropTable("TMS.Expeditor");
            DropTable("TMS.ExpeditorType");
        }
        
        public override void Down()
        {
            CreateTable(
                "TMS.ExpeditorType",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        ExpeditorTypeName = c.String(nullable: false, maxLength: 50),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "TMS.Expeditor",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Initial = c.String(maxLength: 20),
                        ExpeditorName = c.String(maxLength: 50),
                        ExpeditorEmail = c.String(maxLength: 50),
                        Address = c.String(maxLength: 255),
                        PostalCodeID = c.Int(nullable: false),
                        PICID = c.Int(nullable: false),
                        TypeCode = c.Boolean(nullable: false),
                        ExpeditorTypeID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            DropForeignKey("TMS.Vehicle", "ShipperID", "TMS.Partner");
            AlterColumn("TMS.OrderStatus", "CreatedTime", c => c.DateTime(nullable: false));
            DropColumn("TMS.Vehicle", "LastModifiedTime");
            DropColumn("TMS.Vehicle", "LastModifiedBy");
            DropColumn("TMS.Vehicle", "CreatedTime");
            DropColumn("TMS.Vehicle", "CreatedBy");
            DropColumn("TMS.Pool", "LastModifiedTime");
            DropColumn("TMS.Pool", "LastModifiedBy");
            DropColumn("TMS.Pool", "CreatedTime");
            DropColumn("TMS.Pool", "CreatedBy");
            DropColumn("TMS.PIC", "LastModifiedTime");
            DropColumn("TMS.PIC", "LastModifiedBy");
            DropColumn("TMS.PIC", "CreatedTime");
            DropColumn("TMS.PIC", "CreatedBy");
            DropColumn("TMS.Driver", "LastModifiedTime");
            DropColumn("TMS.Driver", "LastModifiedBy");
            DropColumn("TMS.Driver", "CreatedTime");
            DropColumn("TMS.Driver", "CreatedBy");
            CreateIndex("TMS.Expeditor", "ExpeditorTypeID");
            CreateIndex("TMS.Expeditor", "PostalCodeID");
            AddForeignKey("TMS.Vehicle", "ShipperID", "TMS.Expeditor", "ID", cascadeDelete: true);
            AddForeignKey("TMS.Expeditor", "PostalCodeID", "TMS.PostalCode", "ID", cascadeDelete: true);
            AddForeignKey("TMS.Expeditor", "ExpeditorTypeID", "TMS.ExpeditorType", "ID", cascadeDelete: true);
        }
    }
}
