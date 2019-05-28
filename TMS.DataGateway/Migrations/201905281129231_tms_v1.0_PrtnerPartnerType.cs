namespace TMS.DataGateway.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class tms_v10_PrtnerPartnerType : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("TMS.Partner", "PartnerTypeID", "TMS.PartnerType");
            DropIndex("TMS.Partner", new[] { "PartnerTypeID" });
            CreateTable(
                "TMS.PartnerPartnerType",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        PartnerId = c.Int(nullable: false),
                        PartnerTypeId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("TMS.Partner", t => t.PartnerId, cascadeDelete: true)
                .ForeignKey("TMS.PartnerType", t => t.PartnerTypeId, cascadeDelete: true)
                .Index(t => t.PartnerId)
                .Index(t => t.PartnerTypeId);
            
            AddColumn("TMS.OrderPartnerDetail", "PartnerTypeId", c => c.Int(nullable: true));
            CreateIndex("TMS.OrderPartnerDetail", "PartnerTypeId");
            AddForeignKey("TMS.OrderPartnerDetail", "PartnerTypeId", "TMS.PartnerType", "ID", cascadeDelete: true);
            DropColumn("TMS.Partner", "PartnerTypeID");
        }
        
        public override void Down()
        {
            AddColumn("TMS.Partner", "PartnerTypeID", c => c.Int(nullable: false));
            DropForeignKey("TMS.PartnerPartnerType", "PartnerTypeId", "TMS.PartnerType");
            DropForeignKey("TMS.PartnerPartnerType", "PartnerId", "TMS.Partner");
            DropForeignKey("TMS.OrderPartnerDetail", "PartnerTypeId", "TMS.PartnerType");
            DropIndex("TMS.PartnerPartnerType", new[] { "PartnerTypeId" });
            DropIndex("TMS.PartnerPartnerType", new[] { "PartnerId" });
            DropIndex("TMS.OrderPartnerDetail", new[] { "PartnerTypeId" });
            DropColumn("TMS.OrderPartnerDetail", "PartnerTypeId");
            DropTable("TMS.PartnerPartnerType");
            CreateIndex("TMS.Partner", "PartnerTypeID");
            AddForeignKey("TMS.Partner", "PartnerTypeID", "TMS.PartnerType", "ID", cascadeDelete: true);
        }
    }
}
