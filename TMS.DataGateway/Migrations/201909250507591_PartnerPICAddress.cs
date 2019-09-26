namespace TMS.DataGateway.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PartnerPICAddress : DbMigration
    {
        public override void Up()
        {
            DropIndex("TMS.PartnerAddress", "PartnerAddress_PartnerID");
            DropIndex("TMS.PartnerAddress", "PartnerAddress_Address");
            DropIndex("TMS.PartnerPIC", "PartnerPIC_PartnerID");
            DropIndex("TMS.PartnerPIC", "PartnerPIC_PICID");
            DropTable("TMS.PartnerAddress");
            DropTable("TMS.PartnerPIC");
        }
        
        public override void Down()
        {
            CreateTable(
                "TMS.PartnerPIC",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        PartnerID = c.Int(nullable: false),
                        PICID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "TMS.PartnerAddress",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        PartnerID = c.Int(nullable: false),
                        Address = c.String(maxLength: 200),
                        Phone = c.String(maxLength: 15),
                        PostalCodeID = c.Int(nullable: false),
                        IsDefault = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateIndex("TMS.PartnerPIC", "PICID", unique: true, name: "PartnerPIC_PICID");
            CreateIndex("TMS.PartnerPIC", "PartnerID", unique: true, name: "PartnerPIC_PartnerID");
            CreateIndex("TMS.PartnerAddress", "Address", name: "PartnerAddress_Address");
            CreateIndex("TMS.PartnerAddress", "PartnerID", unique: true, name: "PartnerAddress_PartnerID");
        }
    }
}
