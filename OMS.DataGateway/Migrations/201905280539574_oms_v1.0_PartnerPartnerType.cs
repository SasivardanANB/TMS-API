namespace OMS.DataGateway.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class oms_v10_PartnerPartnerType : DbMigration
    {
        public override void Up()
        {
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
            
            AddColumn("OMS.OrderPartnerDetail", "PartnerTypeId", c => c.Int(nullable: true));
            CreateIndex("OMS.OrderPartnerDetail", "PartnerTypeId");
            AddForeignKey("OMS.OrderPartnerDetail", "PartnerTypeId", "OMS.PartnerType", "ID", cascadeDelete: true);
            DropColumn("OMS.User", "Email");
        }
        
        public override void Down()
        {
            AddColumn("OMS.User", "Email", c => c.String(maxLength: 25));
            DropForeignKey("OMS.PartnerPartnerType", "PartnerTypeId", "OMS.PartnerType");
            DropForeignKey("OMS.PartnerPartnerType", "PartnerId", "OMS.Partner");
            DropForeignKey("OMS.OrderPartnerDetail", "PartnerTypeId", "OMS.PartnerType");
            DropIndex("OMS.PartnerPartnerType", new[] { "PartnerTypeId" });
            DropIndex("OMS.PartnerPartnerType", new[] { "PartnerId" });
            DropIndex("OMS.OrderPartnerDetail", new[] { "PartnerTypeId" });
            DropColumn("OMS.OrderPartnerDetail", "PartnerTypeId");
            DropTable("OMS.PartnerPartnerType");
        }
    }
}
