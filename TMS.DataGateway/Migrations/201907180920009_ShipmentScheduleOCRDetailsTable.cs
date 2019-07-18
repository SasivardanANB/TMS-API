namespace TMS.DataGateway.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ShipmentScheduleOCRDetailsTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "TMS.ShipmentScheduleOCRDetails",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        EmailFrom = c.String(maxLength: 250),
                        EmailDateTime = c.DateTime(nullable: false),
                        ShipmentScheduleNo = c.String(),
                        DayShipment = c.String(),
                        ShipmentTime = c.String(),
                        VehicleType = c.String(),
                        MainDealerCode = c.String(),
                        MainDealerName = c.String(),
                        ShipToParty = c.String(),
                        MultiDropShipment = c.String(),
                        EstimatedTotalPallet = c.String(),
                        Weight = c.String(),
                        IsProcessed = c.Boolean(nullable: false),
                        IsOrderCreated = c.Boolean(nullable: false),
                        ImageGuid = c.String(),
                        ProcessedDateTime = c.DateTime(nullable: false),
                        ProcessedBy = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
        }
        
        public override void Down()
        {
            DropTable("TMS.ShipmentScheduleOCRDetails");
        }
    }
}
