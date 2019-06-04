namespace TMS.DataGateway.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModelLenValidations : DbMigration
    {
        public override void Up()
        {
            DropIndex("TMS.BusinessArea", "BusinessArea_BusinessAreaCode");
            DropIndex("TMS.PartnerType", "PartnerType_PartnerTypeCode");
            DropIndex("TMS.OrderPointType", "OrderPointType_PartnerRoleCode");
            DropIndex("TMS.PartnerAddress", "PartnerAddress_Address");
            AlterColumn("TMS.Application", "ApplicationName", c => c.String(maxLength: 100));
            AlterColumn("TMS.BusinessArea", "BusinessAreaCode", c => c.String(nullable: false, maxLength: 4));
            AlterColumn("TMS.Driver", "UserName", c => c.String(nullable: false, maxLength: 30));
            AlterColumn("TMS.Driver", "FirstName", c => c.String(nullable: false, maxLength: 30));
            AlterColumn("TMS.Driver", "LastName", c => c.String(nullable: false, maxLength: 30));
            AlterColumn("TMS.Driver", "DriverAddress", c => c.String(maxLength: 200));
            AlterColumn("TMS.Driver", "Password", c => c.String(nullable: false, maxLength: 30));
            AlterColumn("TMS.G2G", "G2GName", c => c.String(maxLength: 50));
            AlterColumn("TMS.GateInGateOut", "Info", c => c.String(maxLength: 120));
            AlterColumn("TMS.OrderDetail", "Sender", c => c.String(maxLength: 50));
            AlterColumn("TMS.OrderDetail", "Receiver", c => c.String(maxLength: 50));
            AlterColumn("TMS.OrderDetail", "Dimension", c => c.String(maxLength: 11));
            AlterColumn("TMS.OrderDetail", "Instruction", c => c.String(maxLength: 200));
            AlterColumn("TMS.OrderDetail", "ShippingListNo", c => c.String(maxLength: 20));
            AlterColumn("TMS.OrderHeader", "LegecyOrderNo", c => c.String(maxLength: 15));
            AlterColumn("TMS.OrderHeader", "VehicleShipment", c => c.String(maxLength: 50));
            AlterColumn("TMS.OrderHeader", "DriverNo", c => c.String(maxLength: 12));
            AlterColumn("TMS.OrderHeader", "DriverName", c => c.String(maxLength: 60));
            AlterColumn("TMS.OrderHeader", "VehicleNo", c => c.String(maxLength: 12));
            AlterColumn("TMS.OrderHeader", "OrderWeightUM", c => c.String(maxLength: 5));
            AlterColumn("TMS.Partner", "PartnerName", c => c.String(maxLength: 30));
            AlterColumn("TMS.Partner", "PartnerAddress", c => c.String(maxLength: 200));
            AlterColumn("TMS.Partner", "PartnerInitial", c => c.String(maxLength: 30));
            AlterColumn("TMS.Partner", "PartnerEmail", c => c.String(maxLength: 50));
            AlterColumn("TMS.PIC", "PICName", c => c.String(maxLength: 60));
            AlterColumn("TMS.PIC", "PICPassword", c => c.String(maxLength: 30));
            AlterColumn("TMS.PartnerType", "PartnerTypeCode", c => c.String(nullable: false, maxLength: 4));
            AlterColumn("TMS.PartnerType", "PartnerTypeDescription", c => c.String(maxLength: 50));
            AlterColumn("TMS.OrderStatus", "OrderStatusCode", c => c.String(maxLength: 4));
            AlterColumn("TMS.OrderStatus", "OrderStatusValue", c => c.String(maxLength: 50));
            AlterColumn("TMS.OrderStatusHistory", "Remarks", c => c.String(maxLength: 50));
            AlterColumn("TMS.OrderType", "OrderTypeDescription", c => c.String(maxLength: 50));
            AlterColumn("TMS.PackingSheet", "ShippingListNo", c => c.String(maxLength: 20));
            AlterColumn("TMS.PackingSheet", "PackingSheetNo", c => c.String(maxLength: 50));
            AlterColumn("TMS.PartnerAddress", "Phone", c => c.String(maxLength: 15));
            AlterColumn("TMS.Pool", "Address", c => c.String(maxLength: 200));
            AlterColumn("TMS.ShipmentSAP", "ShipmentSAPNo", c => c.String(maxLength: 20));
            AlterColumn("TMS.User", "UserName", c => c.String(nullable: false, maxLength: 30));
            AlterColumn("TMS.User", "Password", c => c.String(maxLength: 30));
            AlterColumn("TMS.User", "FirstName", c => c.String(maxLength: 30));
            AlterColumn("TMS.User", "LastName", c => c.String(maxLength: 30));
            AlterColumn("TMS.Vehicle", "MaxDimension", c => c.String(maxLength: 11));
            AlterColumn("TMS.VehicleType", "VehicleTypeDescription", c => c.String(maxLength: 50));
            CreateIndex("TMS.BusinessArea", "BusinessAreaCode", unique: true, name: "BusinessArea_BusinessAreaCode");
            CreateIndex("TMS.PartnerType", "PartnerTypeCode", unique: true, name: "PartnerType_PartnerTypeCode");
            CreateIndex("TMS.PartnerAddress", "Address", name: "PartnerAddress_Address");
            DropTable("TMS.OrderPointType");
        }
        
        public override void Down()
        {
            CreateTable(
                "TMS.OrderPointType",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        PartnerRoleCode = c.String(nullable: false, maxLength: 10),
                        PartnerRoleDescription = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
            DropIndex("TMS.PartnerAddress", "PartnerAddress_Address");
            DropIndex("TMS.PartnerType", "PartnerType_PartnerTypeCode");
            DropIndex("TMS.BusinessArea", "BusinessArea_BusinessAreaCode");
            AlterColumn("TMS.VehicleType", "VehicleTypeDescription", c => c.String());
            AlterColumn("TMS.Vehicle", "MaxDimension", c => c.String(maxLength: 12));
            AlterColumn("TMS.User", "LastName", c => c.String());
            AlterColumn("TMS.User", "FirstName", c => c.String());
            AlterColumn("TMS.User", "Password", c => c.String());
            AlterColumn("TMS.User", "UserName", c => c.String(nullable: false, maxLength: 50));
            AlterColumn("TMS.ShipmentSAP", "ShipmentSAPNo", c => c.String());
            AlterColumn("TMS.Pool", "Address", c => c.String(maxLength: 255));
            AlterColumn("TMS.PartnerAddress", "Phone", c => c.String());
            AlterColumn("TMS.PackingSheet", "PackingSheetNo", c => c.String());
            AlterColumn("TMS.PackingSheet", "ShippingListNo", c => c.String());
            AlterColumn("TMS.OrderType", "OrderTypeDescription", c => c.String(maxLength: 30));
            AlterColumn("TMS.OrderStatusHistory", "Remarks", c => c.String());
            AlterColumn("TMS.OrderStatus", "OrderStatusValue", c => c.String());
            AlterColumn("TMS.OrderStatus", "OrderStatusCode", c => c.String());
            AlterColumn("TMS.PartnerType", "PartnerTypeDescription", c => c.String());
            AlterColumn("TMS.PartnerType", "PartnerTypeCode", c => c.String(nullable: false, maxLength: 10));
            AlterColumn("TMS.PIC", "PICPassword", c => c.String(maxLength: 50));
            AlterColumn("TMS.PIC", "PICName", c => c.String(maxLength: 50));
            AlterColumn("TMS.Partner", "PartnerEmail", c => c.String());
            AlterColumn("TMS.Partner", "PartnerInitial", c => c.String());
            AlterColumn("TMS.Partner", "PartnerAddress", c => c.String());
            AlterColumn("TMS.Partner", "PartnerName", c => c.String());
            AlterColumn("TMS.OrderHeader", "OrderWeightUM", c => c.String());
            AlterColumn("TMS.OrderHeader", "VehicleNo", c => c.String());
            AlterColumn("TMS.OrderHeader", "DriverName", c => c.String());
            AlterColumn("TMS.OrderHeader", "DriverNo", c => c.String());
            AlterColumn("TMS.OrderHeader", "VehicleShipment", c => c.String());
            AlterColumn("TMS.OrderHeader", "LegecyOrderNo", c => c.String());
            AlterColumn("TMS.OrderDetail", "ShippingListNo", c => c.String());
            AlterColumn("TMS.OrderDetail", "Instruction", c => c.String());
            AlterColumn("TMS.OrderDetail", "Dimension", c => c.String());
            AlterColumn("TMS.OrderDetail", "Receiver", c => c.String());
            AlterColumn("TMS.OrderDetail", "Sender", c => c.String());
            AlterColumn("TMS.GateInGateOut", "Info", c => c.String());
            AlterColumn("TMS.G2G", "G2GName", c => c.String());
            AlterColumn("TMS.Driver", "Password", c => c.String(nullable: false, maxLength: 20));
            AlterColumn("TMS.Driver", "DriverAddress", c => c.String(maxLength: 255));
            AlterColumn("TMS.Driver", "LastName", c => c.String(nullable: false, maxLength: 15));
            AlterColumn("TMS.Driver", "FirstName", c => c.String(nullable: false, maxLength: 15));
            AlterColumn("TMS.Driver", "UserName", c => c.String(nullable: false, maxLength: 15));
            AlterColumn("TMS.BusinessArea", "BusinessAreaCode", c => c.String(nullable: false, maxLength: 5));
            AlterColumn("TMS.Application", "ApplicationName", c => c.String());
            CreateIndex("TMS.PartnerAddress", "Address", unique: true, name: "PartnerAddress_Address");
            CreateIndex("TMS.OrderPointType", "PartnerRoleCode", unique: true, name: "OrderPointType_PartnerRoleCode");
            CreateIndex("TMS.PartnerType", "PartnerTypeCode", unique: true, name: "PartnerType_PartnerTypeCode");
            CreateIndex("TMS.BusinessArea", "BusinessAreaCode", unique: true, name: "BusinessArea_BusinessAreaCode");
        }
    }
}
