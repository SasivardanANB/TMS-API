namespace TMS.DataGateway.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MDTransporterMappingTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "TMS.MDBusinessAreaMapping",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        MainDealerCode = c.String(),
                        BusinessAreaCode = c.String(),
                        CreatedBy = c.String(maxLength: 100),
                        CreatedTime = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(maxLength: 100),
                        LastModifiedTime = c.DateTime(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "TMS.MDTransporterMapping",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        DestinationPartnerID = c.Int(nullable: false),
                        TransporterID = c.Int(nullable: false),
                        Priority = c.Int(nullable: false),
                        CreatedBy = c.String(maxLength: 100),
                        CreatedTime = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(maxLength: 100),
                        LastModifiedTime = c.DateTime(),
                    })
                .PrimaryKey(t => t.ID);
            
        }
        
        public override void Down()
        {
            DropTable("TMS.MDTransporterMapping");
            DropTable("TMS.MDBusinessAreaMapping");
        }
    }
}
