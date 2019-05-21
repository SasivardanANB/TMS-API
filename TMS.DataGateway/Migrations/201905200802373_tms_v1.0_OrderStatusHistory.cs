namespace TMS.DataGateway.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class tms_v10_OrderStatusHistory : DbMigration
    {
        public override void Up()
        {
            DropIndex("TMS.OrderStatusHistory", "OrderStatusHistory_OrderHeaderID");
            DropIndex("TMS.OrderStatusHistory", "OrderStatusHistory_OrderStatus");
            DropIndex("TMS.OrderStatusHistory", "OrderStatusHistory_StepNo");
            AddColumn("TMS.OrderStatusHistory", "OrderDetailID", c => c.Int(nullable: false));
            AddColumn("TMS.OrderStatusHistory", "OrderStatusID", c => c.Int(nullable: false));
            AddColumn("TMS.OrderStatusHistory", "StatusDate", c => c.DateTime(nullable: false));
            AddColumn("TMS.OrderStatusHistory", "Remarks", c => c.String());
            CreateIndex("TMS.OrderStatusHistory", "OrderDetailID");
            CreateIndex("TMS.OrderStatusHistory", "OrderStatusID");
            AddForeignKey("TMS.OrderStatusHistory", "OrderDetailID", "TMS.OrderDetail", "ID", cascadeDelete: true);
            AddForeignKey("TMS.OrderStatusHistory", "OrderStatusID", "TMS.OrderStatus", "ID", cascadeDelete: true);
            DropColumn("TMS.OrderStatusHistory", "OrderHeaderID");
            DropColumn("TMS.OrderStatusHistory", "OrderStatus");
            DropColumn("TMS.OrderStatusHistory", "StepNo");
            DropColumn("TMS.OrderStatusHistory", "IsOptional");
            DropColumn("TMS.OrderStatusHistory", "CreatedTime");
            DropColumn("TMS.OrderStatusHistory", "CreatedBy");
            DropColumn("TMS.OrderStatusHistory", "LastModififiedTime");
            DropColumn("TMS.OrderStatusHistory", "LastModifiedBy");
        }
        
        public override void Down()
        {
            AddColumn("TMS.OrderStatusHistory", "LastModifiedBy", c => c.DateTime(nullable: false));
            AddColumn("TMS.OrderStatusHistory", "LastModififiedTime", c => c.DateTime(nullable: false));
            AddColumn("TMS.OrderStatusHistory", "CreatedBy", c => c.DateTime(nullable: false));
            AddColumn("TMS.OrderStatusHistory", "CreatedTime", c => c.DateTime(nullable: false));
            AddColumn("TMS.OrderStatusHistory", "IsOptional", c => c.Int(nullable: false));
            AddColumn("TMS.OrderStatusHistory", "StepNo", c => c.Int(nullable: false));
            AddColumn("TMS.OrderStatusHistory", "OrderStatus", c => c.Int(nullable: false));
            AddColumn("TMS.OrderStatusHistory", "OrderHeaderID", c => c.Int(nullable: false));
            DropForeignKey("TMS.OrderStatusHistory", "OrderStatusID", "TMS.OrderStatus");
            DropForeignKey("TMS.OrderStatusHistory", "OrderDetailID", "TMS.OrderDetail");
            DropIndex("TMS.OrderStatusHistory", new[] { "OrderStatusID" });
            DropIndex("TMS.OrderStatusHistory", new[] { "OrderDetailID" });
            DropColumn("TMS.OrderStatusHistory", "Remarks");
            DropColumn("TMS.OrderStatusHistory", "StatusDate");
            DropColumn("TMS.OrderStatusHistory", "OrderStatusID");
            DropColumn("TMS.OrderStatusHistory", "OrderDetailID");
            CreateIndex("TMS.OrderStatusHistory", "StepNo", unique: true, name: "OrderStatusHistory_StepNo");
            CreateIndex("TMS.OrderStatusHistory", "OrderStatus", unique: true, name: "OrderStatusHistory_OrderStatus");
            CreateIndex("TMS.OrderStatusHistory", "OrderHeaderID", unique: true, name: "OrderStatusHistory_OrderHeaderID");
        }
    }
}
