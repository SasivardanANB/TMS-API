namespace TMS.DataGateway.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class tms_v10_Orders : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("TMS.Vehicle", "ShipperID", "TMS.Partner");
            DropIndex("TMS.OrderDetail", "OrderDetail_OrderHeaderID");
            DropIndex("TMS.OrderDetail", "OrderDetail_ItemNo");
            DropIndex("TMS.OrderHeader", "OrderHeader_TipeOrder");
            DropIndex("TMS.OrderHeader", "OrderHeader_OrderNo");
            DropIndex("TMS.OrderPartnerDetail", "OrderPartnerDetail_OrderDetailID");
            DropIndex("TMS.OrderPartnerDetail", "OrderPartnerDetail_PartnerTypeID");
            DropIndex("TMS.OrderPartnerDetail", "OrderPartnerDetail_PartnerID");
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
                .PrimaryKey(t => t.ID)
                .ForeignKey("TMS.ExpeditorType", t => t.ExpeditorTypeID, cascadeDelete: true)
                .ForeignKey("TMS.PostalCode", t => t.PostalCodeID, cascadeDelete: true)
                .Index(t => t.PostalCodeID)
                .Index(t => t.ExpeditorTypeID);
            
            CreateTable(
                "TMS.ExpeditorType",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        ExpeditorTypeName = c.String(nullable: false, maxLength: 50),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "TMS.OrderStatus",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        OrderStatusCode = c.String(),
                        OrderStatusValue = c.String(),
                        CreatedBy = c.String(),
                        CreatedTime = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedTime = c.DateTime(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "TMS.PackingSheet",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        OrderDetailID = c.Int(nullable: false),
                        PackingSheetNo = c.String(),
                        CreatedBy = c.String(),
                        CreatedTime = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedTime = c.DateTime(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("TMS.OrderDetail", t => t.OrderDetailID, cascadeDelete: true)
                .Index(t => t.OrderDetailID);
            
            CreateTable(
                "TMS.ShipmentSAP",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        OrderDetailID = c.Int(nullable: false),
                        ShipmentSAPNo = c.String(),
                        CreatedBy = c.String(),
                        CreatedTime = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedTime = c.DateTime(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("TMS.OrderDetail", t => t.OrderDetailID, cascadeDelete: true)
                .Index(t => t.OrderDetailID);
            
            AddColumn("TMS.OrderDetail", "SequenceNo", c => c.Int(nullable: false));
            AddColumn("TMS.OrderDetail", "Sender", c => c.String());
            AddColumn("TMS.OrderDetail", "Receiver", c => c.String());
            AddColumn("TMS.OrderDetail", "Dimension", c => c.String());
            AddColumn("TMS.OrderDetail", "TotalPallet", c => c.Int(nullable: false));
            AddColumn("TMS.OrderDetail", "Instruction", c => c.String());
            AddColumn("TMS.OrderDetail", "ShippingListNo", c => c.String());
            AddColumn("TMS.OrderDetail", "TotalCollie", c => c.Int(nullable: false));
            AddColumn("TMS.OrderDetail", "CreatedBy", c => c.String());
            AddColumn("TMS.OrderDetail", "CreatedTime", c => c.DateTime(nullable: false));
            AddColumn("TMS.OrderDetail", "LastModifiedBy", c => c.String());
            AddColumn("TMS.OrderDetail", "LastModifiedTime", c => c.DateTime());
            AddColumn("TMS.OrderHeader", "LegecyOrderNo", c => c.String());
            AddColumn("TMS.OrderHeader", "OrderType", c => c.Int(nullable: false));
            AddColumn("TMS.OrderHeader", "FleetType", c => c.Int(nullable: false));
            AddColumn("TMS.OrderHeader", "VehicleShipment", c => c.String());
            AddColumn("TMS.OrderHeader", "DriverNo", c => c.String());
            AddColumn("TMS.OrderHeader", "DriverName", c => c.String());
            AddColumn("TMS.OrderHeader", "VehicleNo", c => c.String());
            AddColumn("TMS.OrderHeader", "OrderWeight", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("TMS.OrderHeader", "OrderWeightUM", c => c.String());
            AddColumn("TMS.OrderHeader", "EstimationShipmentDate", c => c.DateTime(nullable: false));
            AddColumn("TMS.OrderHeader", "ActualShipmentDate", c => c.DateTime(nullable: false));
            AddColumn("TMS.OrderHeader", "IsActive", c => c.Boolean(nullable: false));
            AddColumn("TMS.OrderHeader", "CreatedBy", c => c.String());
            AddColumn("TMS.OrderHeader", "CreatedTime", c => c.DateTime(nullable: false));
            AddColumn("TMS.OrderHeader", "LastModifiedBy", c => c.String());
            AddColumn("TMS.OrderHeader", "LastModifiedTime", c => c.DateTime());
            AddColumn("TMS.OrderPartnerDetail", "CreatedBy", c => c.String());
            AddColumn("TMS.OrderPartnerDetail", "CreatedTime", c => c.DateTime(nullable: false));
            AddColumn("TMS.OrderPartnerDetail", "LastModifiedBy", c => c.String());
            AddColumn("TMS.OrderPartnerDetail", "LastModifiedTime", c => c.DateTime());
            AddColumn("TMS.Partner", "IsActive", c => c.Boolean(nullable: false));
            AlterColumn("TMS.OrderHeader", "BusinessAreaId", c => c.Int(nullable: false));
            AlterColumn("TMS.OrderHeader", "OrderNo", c => c.String(nullable: false, maxLength: 20));
            AlterColumn("TMS.OrderPartnerDetail", "IsParent", c => c.Boolean(nullable: false));
            AlterColumn("TMS.OrderPartnerDetail", "IsOriginal", c => c.Boolean(nullable: false));
            CreateIndex("TMS.OrderDetail", "OrderHeaderID");
            CreateIndex("TMS.OrderHeader", "BusinessAreaId");
            CreateIndex("TMS.OrderHeader", "OrderNo", unique: true, name: "OrderHeader_OrderNo");
            CreateIndex("TMS.OrderPartnerDetail", "OrderDetailID");
            CreateIndex("TMS.OrderPartnerDetail", "PartnerID");
            AddForeignKey("TMS.OrderHeader", "BusinessAreaId", "TMS.BusinessArea", "ID", cascadeDelete: true);
            AddForeignKey("TMS.OrderDetail", "OrderHeaderID", "TMS.OrderHeader", "ID", cascadeDelete: true);
            AddForeignKey("TMS.OrderPartnerDetail", "OrderDetailID", "TMS.OrderDetail", "ID", cascadeDelete: true);
            AddForeignKey("TMS.OrderPartnerDetail", "PartnerID", "TMS.Partner", "ID", cascadeDelete: true);
            AddForeignKey("TMS.Vehicle", "ShipperID", "TMS.Expeditor", "ID", cascadeDelete: true);
            DropColumn("TMS.Driver", "CreatedBy");
            DropColumn("TMS.Driver", "CreatedTime");
            DropColumn("TMS.Driver", "LastModifiedBy");
            DropColumn("TMS.Driver", "LastModifiedTime");
            DropColumn("TMS.OrderDetail", "ItemNo");
            DropColumn("TMS.OrderDetail", "Pengirim");
            DropColumn("TMS.OrderDetail", "Penerima");
            DropColumn("TMS.OrderDetail", "Instruksi");
            DropColumn("TMS.OrderDetail", "EstimatedArrivalTime");
            DropColumn("TMS.OrderDetail", "ActualArrivalTime");
            DropColumn("TMS.OrderHeader", "CompanyCodeID");
            DropColumn("TMS.OrderHeader", "TipeOrder");
            DropColumn("TMS.OrderHeader", "EstimatedPickupTime");
            DropColumn("TMS.OrderHeader", "ActualPickupTime");
            DropColumn("TMS.OrderHeader", "EstimatedArrivalTime");
            DropColumn("TMS.OrderHeader", "ActualArrivalTime");
            DropColumn("TMS.OrderHeader", "TotalPrice");
            DropColumn("TMS.OrderHeader", "Sender");
            DropColumn("TMS.OrderHeader", "Receiver");
            DropColumn("TMS.OrderHeader", "Instruction");
            DropColumn("TMS.OrderHeader", "VehicleType");
            DropColumn("TMS.OrderHeader", "PoliceNo");
            DropColumn("TMS.OrderHeader", "TotalOfWeight");
            DropColumn("TMS.OrderPartnerDetail", "PartnerTypeID");
            DropColumn("TMS.OrderPartnerDetail", "ParentID");
            DropColumn("TMS.OrderPartnerDetail", "CustomerName");
            DropColumn("TMS.OrderPartnerDetail", "CustomerPhone");
            DropColumn("TMS.OrderPartnerDetail", "CustomerAddress");
            DropColumn("TMS.OrderPartnerDetail", "Longitude");
            DropColumn("TMS.OrderPartnerDetail", "Lattitude");
            DropColumn("TMS.PIC", "CreatedBy");
            DropColumn("TMS.PIC", "CreatedTime");
            DropColumn("TMS.PIC", "LastModifiedBy");
            DropColumn("TMS.PIC", "LastModifiedTime");
            DropColumn("TMS.Pool", "CreatedBy");
            DropColumn("TMS.Pool", "CreatedTime");
            DropColumn("TMS.Pool", "LastModifiedBy");
            DropColumn("TMS.Pool", "LastModifiedTime");
            DropColumn("TMS.Vehicle", "CreatedBy");
            DropColumn("TMS.Vehicle", "CreatedTime");
            DropColumn("TMS.Vehicle", "LastModifiedBy");
            DropColumn("TMS.Vehicle", "LastModifiedTime");
        }
        
        public override void Down()
        {
            AddColumn("TMS.Vehicle", "LastModifiedTime", c => c.DateTime());
            AddColumn("TMS.Vehicle", "LastModifiedBy", c => c.String());
            AddColumn("TMS.Vehicle", "CreatedTime", c => c.DateTime(nullable: false));
            AddColumn("TMS.Vehicle", "CreatedBy", c => c.String());
            AddColumn("TMS.Pool", "LastModifiedTime", c => c.DateTime());
            AddColumn("TMS.Pool", "LastModifiedBy", c => c.String());
            AddColumn("TMS.Pool", "CreatedTime", c => c.DateTime(nullable: false));
            AddColumn("TMS.Pool", "CreatedBy", c => c.String());
            AddColumn("TMS.PIC", "LastModifiedTime", c => c.DateTime());
            AddColumn("TMS.PIC", "LastModifiedBy", c => c.String());
            AddColumn("TMS.PIC", "CreatedTime", c => c.DateTime(nullable: false));
            AddColumn("TMS.PIC", "CreatedBy", c => c.String());
            AddColumn("TMS.OrderPartnerDetail", "Lattitude", c => c.String());
            AddColumn("TMS.OrderPartnerDetail", "Longitude", c => c.String());
            AddColumn("TMS.OrderPartnerDetail", "CustomerAddress", c => c.String(maxLength: 200));
            AddColumn("TMS.OrderPartnerDetail", "CustomerPhone", c => c.String());
            AddColumn("TMS.OrderPartnerDetail", "CustomerName", c => c.String());
            AddColumn("TMS.OrderPartnerDetail", "ParentID", c => c.Int(nullable: false));
            AddColumn("TMS.OrderPartnerDetail", "PartnerTypeID", c => c.Int(nullable: false));
            AddColumn("TMS.OrderHeader", "TotalOfWeight", c => c.Int(nullable: false));
            AddColumn("TMS.OrderHeader", "PoliceNo", c => c.String());
            AddColumn("TMS.OrderHeader", "VehicleType", c => c.String());
            AddColumn("TMS.OrderHeader", "Instruction", c => c.String());
            AddColumn("TMS.OrderHeader", "Receiver", c => c.String());
            AddColumn("TMS.OrderHeader", "Sender", c => c.String());
            AddColumn("TMS.OrderHeader", "TotalPrice", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("TMS.OrderHeader", "ActualArrivalTime", c => c.DateTime(nullable: false));
            AddColumn("TMS.OrderHeader", "EstimatedArrivalTime", c => c.DateTime(nullable: false));
            AddColumn("TMS.OrderHeader", "ActualPickupTime", c => c.DateTime(nullable: false));
            AddColumn("TMS.OrderHeader", "EstimatedPickupTime", c => c.DateTime(nullable: false));
            AddColumn("TMS.OrderHeader", "TipeOrder", c => c.String(maxLength: 10));
            AddColumn("TMS.OrderHeader", "CompanyCodeID", c => c.String(maxLength: 4));
            AddColumn("TMS.OrderDetail", "ActualArrivalTime", c => c.DateTime(nullable: false));
            AddColumn("TMS.OrderDetail", "EstimatedArrivalTime", c => c.DateTime(nullable: false));
            AddColumn("TMS.OrderDetail", "Instruksi", c => c.String());
            AddColumn("TMS.OrderDetail", "Penerima", c => c.String());
            AddColumn("TMS.OrderDetail", "Pengirim", c => c.String());
            AddColumn("TMS.OrderDetail", "ItemNo", c => c.Int(nullable: false));
            AddColumn("TMS.Driver", "LastModifiedTime", c => c.DateTime());
            AddColumn("TMS.Driver", "LastModifiedBy", c => c.String());
            AddColumn("TMS.Driver", "CreatedTime", c => c.DateTime(nullable: false));
            AddColumn("TMS.Driver", "CreatedBy", c => c.String());
            DropForeignKey("TMS.Vehicle", "ShipperID", "TMS.Expeditor");
            DropForeignKey("TMS.ShipmentSAP", "OrderDetailID", "TMS.OrderDetail");
            DropForeignKey("TMS.PackingSheet", "OrderDetailID", "TMS.OrderDetail");
            DropForeignKey("TMS.OrderPartnerDetail", "PartnerID", "TMS.Partner");
            DropForeignKey("TMS.OrderPartnerDetail", "OrderDetailID", "TMS.OrderDetail");
            DropForeignKey("TMS.OrderDetail", "OrderHeaderID", "TMS.OrderHeader");
            DropForeignKey("TMS.OrderHeader", "BusinessAreaId", "TMS.BusinessArea");
            DropForeignKey("TMS.Expeditor", "PostalCodeID", "TMS.PostalCode");
            DropForeignKey("TMS.Expeditor", "ExpeditorTypeID", "TMS.ExpeditorType");
            DropIndex("TMS.ShipmentSAP", new[] { "OrderDetailID" });
            DropIndex("TMS.PackingSheet", new[] { "OrderDetailID" });
            DropIndex("TMS.OrderPartnerDetail", new[] { "PartnerID" });
            DropIndex("TMS.OrderPartnerDetail", new[] { "OrderDetailID" });
            DropIndex("TMS.OrderHeader", "OrderHeader_OrderNo");
            DropIndex("TMS.OrderHeader", new[] { "BusinessAreaId" });
            DropIndex("TMS.OrderDetail", new[] { "OrderHeaderID" });
            DropIndex("TMS.Expeditor", new[] { "ExpeditorTypeID" });
            DropIndex("TMS.Expeditor", new[] { "PostalCodeID" });
            AlterColumn("TMS.OrderPartnerDetail", "IsOriginal", c => c.Int(nullable: false));
            AlterColumn("TMS.OrderPartnerDetail", "IsParent", c => c.Int(nullable: false));
            AlterColumn("TMS.OrderHeader", "OrderNo", c => c.String(maxLength: 10));
            AlterColumn("TMS.OrderHeader", "BusinessAreaId", c => c.String());
            DropColumn("TMS.Partner", "IsActive");
            DropColumn("TMS.OrderPartnerDetail", "LastModifiedTime");
            DropColumn("TMS.OrderPartnerDetail", "LastModifiedBy");
            DropColumn("TMS.OrderPartnerDetail", "CreatedTime");
            DropColumn("TMS.OrderPartnerDetail", "CreatedBy");
            DropColumn("TMS.OrderHeader", "LastModifiedTime");
            DropColumn("TMS.OrderHeader", "LastModifiedBy");
            DropColumn("TMS.OrderHeader", "CreatedTime");
            DropColumn("TMS.OrderHeader", "CreatedBy");
            DropColumn("TMS.OrderHeader", "IsActive");
            DropColumn("TMS.OrderHeader", "ActualShipmentDate");
            DropColumn("TMS.OrderHeader", "EstimationShipmentDate");
            DropColumn("TMS.OrderHeader", "OrderWeightUM");
            DropColumn("TMS.OrderHeader", "OrderWeight");
            DropColumn("TMS.OrderHeader", "VehicleNo");
            DropColumn("TMS.OrderHeader", "DriverName");
            DropColumn("TMS.OrderHeader", "DriverNo");
            DropColumn("TMS.OrderHeader", "VehicleShipment");
            DropColumn("TMS.OrderHeader", "FleetType");
            DropColumn("TMS.OrderHeader", "OrderType");
            DropColumn("TMS.OrderHeader", "LegecyOrderNo");
            DropColumn("TMS.OrderDetail", "LastModifiedTime");
            DropColumn("TMS.OrderDetail", "LastModifiedBy");
            DropColumn("TMS.OrderDetail", "CreatedTime");
            DropColumn("TMS.OrderDetail", "CreatedBy");
            DropColumn("TMS.OrderDetail", "TotalCollie");
            DropColumn("TMS.OrderDetail", "ShippingListNo");
            DropColumn("TMS.OrderDetail", "Instruction");
            DropColumn("TMS.OrderDetail", "TotalPallet");
            DropColumn("TMS.OrderDetail", "Dimension");
            DropColumn("TMS.OrderDetail", "Receiver");
            DropColumn("TMS.OrderDetail", "Sender");
            DropColumn("TMS.OrderDetail", "SequenceNo");
            DropTable("TMS.ShipmentSAP");
            DropTable("TMS.PackingSheet");
            DropTable("TMS.OrderStatus");
            DropTable("TMS.ExpeditorType");
            DropTable("TMS.Expeditor");
            CreateIndex("TMS.OrderPartnerDetail", "PartnerID", unique: true, name: "OrderPartnerDetail_PartnerID");
            CreateIndex("TMS.OrderPartnerDetail", "PartnerTypeID", unique: true, name: "OrderPartnerDetail_PartnerTypeID");
            CreateIndex("TMS.OrderPartnerDetail", "OrderDetailID", unique: true, name: "OrderPartnerDetail_OrderDetailID");
            CreateIndex("TMS.OrderHeader", "OrderNo", unique: true, name: "OrderHeader_OrderNo");
            CreateIndex("TMS.OrderHeader", "TipeOrder", unique: true, name: "OrderHeader_TipeOrder");
            CreateIndex("TMS.OrderDetail", "ItemNo", unique: true, name: "OrderDetail_ItemNo");
            CreateIndex("TMS.OrderDetail", "OrderHeaderID", unique: true, name: "OrderDetail_OrderHeaderID");
            AddForeignKey("TMS.Vehicle", "ShipperID", "TMS.Partner", "ID", cascadeDelete: true);
        }
    }
}
