namespace TMS.DataGateway.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class tms_v10_DBCleanup : DbMigration
    {
        public override void Up()
        {
            DropIndex("TMS.BillingTripDetail", "BillingTripDetail_BillingHeaderID");
            DropIndex("TMS.BillingTripDetail", "BillingTripDetail_ItemNo");
            DropIndex("TMS.BillingTripDetail", "BillingTripDetail_TripDetailID");
            DropIndex("TMS.BillingTripPartnerDetail", "BillingTripPartnerDetail_BillingDetailID");
            DropIndex("TMS.BillingTripPartnerDetail", "BillingTripPartnerDetail_PartnerTypeID");
            DropIndex("TMS.BillingTripPartnerDetail", "BillingTripPartnerDetail_PartnerID");
            DropIndex("TMS.BillingTrip", "BillingTrip_CompanyCodeID");
            DropIndex("TMS.BillingTrip", "BillingTrip_BusinessAreaID");
            DropIndex("TMS.BillingTrip", "BillingTrip_BillingNo");
            DropIndex("TMS.CancellationReason", "CancellationReason_CancellationReasonCode");
            DropIndex("TMS.CompanyCodeNumberingRange", "CompanyCodeNumberingRange_CompanyCodeCode");
            DropIndex("TMS.CompanyCodeNumberingRange", "CompanyCodeNumberingRange_BusinessAreaCode");
            DropIndex("TMS.CompanyCodeNumberingRange", "CompanyCodeNumberingRange_TransactionTypeCode");
            DropIndex("TMS.TripDetail", "TripDetail_TripHeaderID");
            DropIndex("TMS.TripDetail", "TripDetail_ItemNo");
            DropIndex("TMS.TripHeader", "TripHeader_TripNo");
            DropIndex("TMS.TripPartnerDetail", "TripPartnerDetail_TripDetailID");
            DropIndex("TMS.TripPartnerDetail", "TripPartnerDetail_PartnerID");
            DropIndex("TMS.TripPartnerDetail", "TripPartnerDetail_PartnerTypeID");
            DropIndex("TMS.TripStatus", "TripStatus_TripStatusCode");
            DropIndex("TMS.TripStatusHistory", "TripStatusHistory_TripHeaderID");
            DropIndex("TMS.TripStatusHistory", "TripStatusHistory_TripStatusID");
            DropIndex("TMS.TripStatusHistory", "TripStatusHistory_StepNo");
            DropIndex("TMS.TripType", "TripType_TripTypeCode");
            AddColumn("TMS.OrderDetail", "EstimationShipmentDate", c => c.DateTime(nullable: false));
            AddColumn("TMS.OrderDetail", "ActualShipmentDate", c => c.DateTime(nullable: false));
            DropColumn("TMS.OrderHeader", "EstimationShipmentDate");
            DropColumn("TMS.OrderHeader", "ActualShipmentDate");
            DropTable("TMS.BillingTripDetail");
            DropTable("TMS.BillingTripPartnerDetail");
            DropTable("TMS.BillingTrip");
            DropTable("TMS.CancellationReason");
            DropTable("TMS.CompanyCodeNumberingRange");
            DropTable("TMS.TripDetail");
            DropTable("TMS.TripHeader");
            DropTable("TMS.TripPartnerDetail");
            DropTable("TMS.TripStatus");
            DropTable("TMS.TripStatusHistory");
            DropTable("TMS.TripType");
        }
        
        public override void Down()
        {
            CreateTable(
                "TMS.TripType",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        TripTypeCode = c.String(maxLength: 10),
                        TripTypeDescription = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "TMS.TripStatusHistory",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        TripHeaderID = c.Int(nullable: false),
                        TripStatusID = c.Int(nullable: false),
                        StepNo = c.Int(nullable: false),
                        IsOptional = c.Int(nullable: false),
                        CreatedTime = c.DateTime(nullable: false),
                        CreatedBy = c.DateTime(nullable: false),
                        LastModififiedTime = c.DateTime(nullable: false),
                        LastModifiedBy = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "TMS.TripStatus",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        TripStatusCode = c.String(maxLength: 10),
                        TripStatusDescription = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "TMS.TripPartnerDetail",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        TripDetailID = c.Int(nullable: false),
                        PartnerID = c.Int(nullable: false),
                        PartnerTypeID = c.Int(nullable: false),
                        ParentID = c.Int(nullable: false),
                        IsParent = c.Int(nullable: false),
                        IsOriginal = c.Int(nullable: false),
                        DriverID = c.Int(nullable: false),
                        VehicleTypeID = c.Int(nullable: false),
                        Price = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "TMS.TripHeader",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        BusinessAreaID = c.Int(nullable: false),
                        TripNo = c.String(maxLength: 10),
                        TripStatusID = c.Int(nullable: false),
                        TripTypeID = c.Int(nullable: false),
                        TripDate = c.DateTime(nullable: false),
                        Notes = c.String(),
                        EstimatedTripTime = c.DateTime(nullable: false),
                        ActualTripTime = c.DateTime(nullable: false),
                        TotalWeight = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalKM = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ReferenceTripNo = c.Int(nullable: false),
                        IsParent = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "TMS.TripDetail",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        TripHeaderID = c.Int(nullable: false),
                        ItemNo = c.Int(nullable: false),
                        OrderDetailID = c.Int(nullable: false),
                        Berat = c.Decimal(nullable: false, precision: 18, scale: 2),
                        KM = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Price = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "TMS.CompanyCodeNumberingRange",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        CompanyCodeCode = c.String(nullable: false, maxLength: 4),
                        BusinessAreaCode = c.String(maxLength: 10),
                        IsNumberRange = c.Int(nullable: false),
                        TransactionTypeCode = c.String(maxLength: 10),
                        IsDisplayOnly = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "TMS.CancellationReason",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        CancellationReasonCode = c.String(maxLength: 10),
                        CancellationReasonDescription = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "TMS.BillingTrip",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        CompanyCodeID = c.Int(nullable: false),
                        BusinessAreaID = c.Int(nullable: false),
                        BillingNo = c.String(maxLength: 10),
                        BillingDate = c.DateTime(nullable: false),
                        BillingTypeID = c.Int(nullable: false),
                        BillingStatusID = c.Int(nullable: false),
                        TermsOfPaymentID = c.Int(nullable: false),
                        CancellationReasonID = c.Int(nullable: false),
                        CancellationReasonDescription = c.String(),
                        TotalNetValue = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalVATAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalDiscount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalIncVAT = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "TMS.BillingTripPartnerDetail",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        BillingDetailID = c.Int(nullable: false),
                        PartnerTypeID = c.Int(nullable: false),
                        PartnerID = c.Int(nullable: false),
                        ParentID = c.Int(nullable: false),
                        IsParent = c.Int(nullable: false),
                        IsOriginal = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "TMS.BillingTripDetail",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        BillingHeaderID = c.Int(nullable: false),
                        ItemNo = c.Int(nullable: false),
                        TripDetailID = c.Int(nullable: false),
                        ItemStatus = c.Int(nullable: false),
                        Berat = c.Decimal(nullable: false, precision: 18, scale: 2),
                        KM = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Price = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Discount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        VATAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.ID);
            
            AddColumn("TMS.OrderHeader", "ActualShipmentDate", c => c.DateTime(nullable: false));
            AddColumn("TMS.OrderHeader", "EstimationShipmentDate", c => c.DateTime(nullable: false));
            DropColumn("TMS.OrderDetail", "ActualShipmentDate");
            DropColumn("TMS.OrderDetail", "EstimationShipmentDate");
            CreateIndex("TMS.TripType", "TripTypeCode", unique: true, name: "TripType_TripTypeCode");
            CreateIndex("TMS.TripStatusHistory", "StepNo", unique: true, name: "TripStatusHistory_StepNo");
            CreateIndex("TMS.TripStatusHistory", "TripStatusID", unique: true, name: "TripStatusHistory_TripStatusID");
            CreateIndex("TMS.TripStatusHistory", "TripHeaderID", unique: true, name: "TripStatusHistory_TripHeaderID");
            CreateIndex("TMS.TripStatus", "TripStatusCode", unique: true, name: "TripStatus_TripStatusCode");
            CreateIndex("TMS.TripPartnerDetail", "PartnerTypeID", unique: true, name: "TripPartnerDetail_PartnerTypeID");
            CreateIndex("TMS.TripPartnerDetail", "PartnerID", unique: true, name: "TripPartnerDetail_PartnerID");
            CreateIndex("TMS.TripPartnerDetail", "TripDetailID", unique: true, name: "TripPartnerDetail_TripDetailID");
            CreateIndex("TMS.TripHeader", "TripNo", unique: true, name: "TripHeader_TripNo");
            CreateIndex("TMS.TripDetail", "ItemNo", unique: true, name: "TripDetail_ItemNo");
            CreateIndex("TMS.TripDetail", "TripHeaderID", unique: true, name: "TripDetail_TripHeaderID");
            CreateIndex("TMS.CompanyCodeNumberingRange", "TransactionTypeCode", unique: true, name: "CompanyCodeNumberingRange_TransactionTypeCode");
            CreateIndex("TMS.CompanyCodeNumberingRange", "BusinessAreaCode", unique: true, name: "CompanyCodeNumberingRange_BusinessAreaCode");
            CreateIndex("TMS.CompanyCodeNumberingRange", "CompanyCodeCode", unique: true, name: "CompanyCodeNumberingRange_CompanyCodeCode");
            CreateIndex("TMS.CancellationReason", "CancellationReasonCode", unique: true, name: "CancellationReason_CancellationReasonCode");
            CreateIndex("TMS.BillingTrip", "BillingNo", unique: true, name: "BillingTrip_BillingNo");
            CreateIndex("TMS.BillingTrip", "BusinessAreaID", unique: true, name: "BillingTrip_BusinessAreaID");
            CreateIndex("TMS.BillingTrip", "CompanyCodeID", unique: true, name: "BillingTrip_CompanyCodeID");
            CreateIndex("TMS.BillingTripPartnerDetail", "PartnerID", unique: true, name: "BillingTripPartnerDetail_PartnerID");
            CreateIndex("TMS.BillingTripPartnerDetail", "PartnerTypeID", unique: true, name: "BillingTripPartnerDetail_PartnerTypeID");
            CreateIndex("TMS.BillingTripPartnerDetail", "BillingDetailID", unique: true, name: "BillingTripPartnerDetail_BillingDetailID");
            CreateIndex("TMS.BillingTripDetail", "TripDetailID", unique: true, name: "BillingTripDetail_TripDetailID");
            CreateIndex("TMS.BillingTripDetail", "ItemNo", unique: true, name: "BillingTripDetail_ItemNo");
            CreateIndex("TMS.BillingTripDetail", "BillingHeaderID", unique: true, name: "BillingTripDetail_BillingHeaderID");
        }
    }
}
