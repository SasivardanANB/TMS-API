namespace TMS.DataGateway.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class HargaTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "TMS.Harga",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        TransporterID = c.Int(nullable: false),
                        VechicleTypeID = c.Int(nullable: false),
                        Price = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CreatedBy = c.String(maxLength: 100),
                        CreatedTime = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(maxLength: 100),
                        LastModifiedTime = c.DateTime(),
                    })
                .PrimaryKey(t => t.ID);
            
        }
        
        public override void Down()
        {
            DropTable("TMS.Harga");
        }
    }
}
