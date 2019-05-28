namespace OMS.DataGateway.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class oms_v10_OrderPartnerTypeIdREmoved : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("OMS.Partner", "PartnerTypeID", "OMS.PartnerType");
            DropIndex("OMS.Partner", new[] { "PartnerTypeID" });
            CreateTable(
                "OMS.PartnerPartnerType",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        PartnerId = c.Int(nullable: false),
                        PartnerTypeId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("OMS.Partner", t => t.PartnerId, cascadeDelete: true)
                .ForeignKey("OMS.PartnerType", t => t.PartnerTypeId, cascadeDelete: true)
                .Index(t => t.PartnerId)
                .Index(t => t.PartnerTypeId);
            
            AddColumn("OMS.OrderPartnerDetail", "PartnerTypeId", c => c.Int(nullable: false));
            CreateIndex("OMS.OrderPartnerDetail", "PartnerTypeId");
            AddForeignKey("OMS.OrderPartnerDetail", "PartnerTypeId", "OMS.PartnerType", "ID", cascadeDelete: true);
            DropColumn("OMS.Partner", "PartnerTypeID");
            DropColumn("OMS.User", "Email");
        }
        
        public override void Down()
        {
            AddColumn("OMS.User", "Email", c => c.String(maxLength: 25));
            AddColumn("OMS.Partner", "PartnerTypeID", c => c.Int(nullable: false));
            DropForeignKey("OMS.PartnerPartnerType", "PartnerTypeId", "OMS.PartnerType");
            DropForeignKey("OMS.PartnerPartnerType", "PartnerId", "OMS.Partner");
            DropForeignKey("OMS.OrderPartnerDetail", "PartnerTypeId", "OMS.PartnerType");
            DropIndex("OMS.PartnerPartnerType", new[] { "PartnerTypeId" });
            DropIndex("OMS.PartnerPartnerType", new[] { "PartnerId" });
            DropIndex("OMS.OrderPartnerDetail", new[] { "PartnerTypeId" });
            DropColumn("OMS.OrderPartnerDetail", "PartnerTypeId");
            DropTable("OMS.PartnerPartnerType");
            CreateIndex("OMS.Partner", "PartnerTypeID");
            AddForeignKey("OMS.Partner", "PartnerTypeID", "OMS.PartnerType", "ID", cascadeDelete: true);
        }
    }
}
