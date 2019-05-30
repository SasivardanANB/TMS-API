namespace TMS.DataGateway.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class tmsv10_Unusedtablecleaned : DbMigration
    {
        public override void Up()
        {
            DropIndex("TMS.MappingOrderPartner", "MappingOrderPartner_OrderTypeID");
            DropIndex("TMS.MappingOrderPartner", "MappingOrderPartner_PartnerTypeID");
            DropIndex("TMS.NumberingRange", "NumberingRange_CompanyCodeCode");
            DropIndex("TMS.NumberingRange", "NumberingRange_BusinessAreaCode");
            DropIndex("TMS.NumberingRange", "NumberingRange_TransactionTypeCode");
            DropIndex("TMS.OrderHeaderHSOAdditionalData", "OrderHeaderHSOAdditionalData_OrderHeaderID");
            DropIndex("TMS.OrderTripStatusWorkFlow", "OrderTripStatusWorkFlow_TripTypeID");
            DropIndex("TMS.OrderTripStatusWorkFlow", "OrderTripStatusWorkFlow_TripStatusID");
            DropIndex("TMS.OrderTripStatusWorkFlow", "OrderTripStatusWorkFlow_StepNo");
            DropIndex("TMS.OrderTypeStatusWorkFlow", "OrderTypeStatusWorkFlow_TipeOrderID");
            DropIndex("TMS.OrderTypeStatusWorkFlow", "OrderTypeStatusWorkFlow_OrderStatus");
            DropIndex("TMS.OrderTypeStatusWorkFlow", "OrderTypeStatusWorkFlow_StepNo");
            DropIndex("TMS.PartnerRole", "PartnerRole_PartnerRoleCode");
            DropIndex("TMS.PartnerTypeFunction", "PartnerTypeFunction_PartnerID");
            DropIndex("TMS.PartnerTypeFunction", "PartnerTypeFunction_PartnerTypeID");
            DropIndex("TMS.TermsOfPayment", "TermsOfPayment_TermsOfPaymentCode");
            DropTable("TMS.MappingOrderPartner");
            DropTable("TMS.NumberingRange");
            DropTable("TMS.OrderHeaderHSOAdditionalData");
            DropTable("TMS.OrderTripStatusWorkFlow");
            DropTable("TMS.OrderTypeStatusWorkFlow");
            DropTable("TMS.PartnerRole");
            DropTable("TMS.PartnerTypeFunction");
            DropTable("TMS.TermsOfPayment");
        }
        
        public override void Down()
        {
            CreateTable(
                "TMS.TermsOfPayment",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        TermsOfPaymentCode = c.String(maxLength: 10),
                        TermsOfPaymentDescription = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "TMS.PartnerTypeFunction",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        PartnerID = c.Int(nullable: false),
                        PartnerTypeID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "TMS.PartnerRole",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        PartnerRoleCode = c.String(nullable: false, maxLength: 10),
                        PartnerRoleDescription = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "TMS.OrderTypeStatusWorkFlow",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        TipeOrderID = c.Int(nullable: false),
                        OrderStatus = c.Int(nullable: false),
                        StepNo = c.Int(nullable: false),
                        IsOptional = c.Int(nullable: false),
                        isTrackable = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "TMS.OrderTripStatusWorkFlow",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        TripTypeID = c.Int(nullable: false),
                        TripStatusID = c.Int(nullable: false),
                        StepNo = c.Int(nullable: false),
                        IsOptional = c.Int(nullable: false),
                        IsTrackable = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "TMS.OrderHeaderHSOAdditionalData",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        OrderHeaderID = c.Int(nullable: false),
                        ShipmentIDAHM = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "TMS.NumberingRange",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        CompanyCodeCode = c.String(nullable: false, maxLength: 10),
                        BusinessAreaCode = c.String(maxLength: 10),
                        TransactionTypeCode = c.String(maxLength: 10),
                        Prefix = c.String(),
                        StartNumber = c.Int(nullable: false),
                        EndNumber = c.Int(nullable: false),
                        LastNumber = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "TMS.MappingOrderPartner",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        OrderTypeID = c.Int(nullable: false),
                        PartnerTypeID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateIndex("TMS.TermsOfPayment", "TermsOfPaymentCode", unique: true, name: "TermsOfPayment_TermsOfPaymentCode");
            CreateIndex("TMS.PartnerTypeFunction", "PartnerTypeID", unique: true, name: "PartnerTypeFunction_PartnerTypeID");
            CreateIndex("TMS.PartnerTypeFunction", "PartnerID", unique: true, name: "PartnerTypeFunction_PartnerID");
            CreateIndex("TMS.PartnerRole", "PartnerRoleCode", unique: true, name: "PartnerRole_PartnerRoleCode");
            CreateIndex("TMS.OrderTypeStatusWorkFlow", "StepNo", unique: true, name: "OrderTypeStatusWorkFlow_StepNo");
            CreateIndex("TMS.OrderTypeStatusWorkFlow", "OrderStatus", unique: true, name: "OrderTypeStatusWorkFlow_OrderStatus");
            CreateIndex("TMS.OrderTypeStatusWorkFlow", "TipeOrderID", unique: true, name: "OrderTypeStatusWorkFlow_TipeOrderID");
            CreateIndex("TMS.OrderTripStatusWorkFlow", "StepNo", unique: true, name: "OrderTripStatusWorkFlow_StepNo");
            CreateIndex("TMS.OrderTripStatusWorkFlow", "TripStatusID", unique: true, name: "OrderTripStatusWorkFlow_TripStatusID");
            CreateIndex("TMS.OrderTripStatusWorkFlow", "TripTypeID", unique: true, name: "OrderTripStatusWorkFlow_TripTypeID");
            CreateIndex("TMS.OrderHeaderHSOAdditionalData", "OrderHeaderID", unique: true, name: "OrderHeaderHSOAdditionalData_OrderHeaderID");
            CreateIndex("TMS.NumberingRange", "TransactionTypeCode", unique: true, name: "NumberingRange_TransactionTypeCode");
            CreateIndex("TMS.NumberingRange", "BusinessAreaCode", unique: true, name: "NumberingRange_BusinessAreaCode");
            CreateIndex("TMS.NumberingRange", "CompanyCodeCode", unique: true, name: "NumberingRange_CompanyCodeCode");
            CreateIndex("TMS.MappingOrderPartner", "PartnerTypeID", unique: true, name: "MappingOrderPartner_PartnerTypeID");
            CreateIndex("TMS.MappingOrderPartner", "OrderTypeID", unique: true, name: "MappingOrderPartner_OrderTypeID");
        }
    }
}
