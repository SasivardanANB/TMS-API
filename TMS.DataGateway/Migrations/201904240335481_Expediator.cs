namespace TMS.DataGateway.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Expediator : DbMigration
    {
        public override void Up()
        {
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
                "TMS.RoleMenuActivity",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        RoleMenuID = c.Int(nullable: false),
                        ActivityID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("TMS.Activity", t => t.ActivityID, cascadeDelete: true)
                .ForeignKey("TMS.RoleMenu", t => t.RoleMenuID, cascadeDelete: true)
                .Index(t => t.RoleMenuID)
                .Index(t => t.ActivityID);
            
            CreateTable(
                "TMS.RoleMenu",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        RoleID = c.Int(nullable: false),
                        MenuID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("TMS.Menu", t => t.MenuID, cascadeDelete: true)
                .ForeignKey("TMS.Role", t => t.RoleID, cascadeDelete: true)
                .Index(t => t.RoleID)
                .Index(t => t.MenuID);
            
            CreateTable(
                "TMS.Token",
                c => new
                    {
                        TokenID = c.Int(nullable: false, identity: true),
                        TokenKey = c.String(),
                        IssuedOn = c.DateTime(nullable: false),
                        ExpiresOn = c.DateTime(nullable: false),
                        CreatedOn = c.DateTime(nullable: false),
                        UserID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.TokenID)
                .ForeignKey("TMS.User", t => t.UserID, cascadeDelete: true)
                .Index(t => t.UserID);
            
            CreateTable(
                "TMS.User",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        UserName = c.String(nullable: false, maxLength: 50),
                        Password = c.String(),
                        FirstName = c.String(),
                        LastName = c.String(),
                        RoleID = c.Int(),
                        BusinessAreaID = c.Int(),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("TMS.BusinessArea", t => t.BusinessAreaID)
                .ForeignKey("TMS.Role", t => t.RoleID)
                .Index(t => t.UserName, unique: true, name: "User_UserName")
                .Index(t => t.RoleID)
                .Index(t => t.BusinessAreaID);
            
            CreateTable(
                "TMS.UserApplication",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        ApplicationID = c.Int(nullable: false),
                        UserID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("TMS.Application", t => t.ApplicationID, cascadeDelete: true)
                .ForeignKey("TMS.User", t => t.UserID, cascadeDelete: true)
                .Index(t => t.ApplicationID)
                .Index(t => t.UserID);
            
            AddColumn("TMS.Role", "IsActive", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropForeignKey("TMS.UserApplication", "UserID", "TMS.User");
            DropForeignKey("TMS.UserApplication", "ApplicationID", "TMS.Application");
            DropForeignKey("TMS.Token", "UserID", "TMS.User");
            DropForeignKey("TMS.User", "RoleID", "TMS.Role");
            DropForeignKey("TMS.User", "BusinessAreaID", "TMS.BusinessArea");
            DropForeignKey("TMS.RoleMenuActivity", "RoleMenuID", "TMS.RoleMenu");
            DropForeignKey("TMS.RoleMenu", "RoleID", "TMS.Role");
            DropForeignKey("TMS.RoleMenu", "MenuID", "TMS.Menu");
            DropForeignKey("TMS.RoleMenuActivity", "ActivityID", "TMS.Activity");
            DropForeignKey("TMS.Expeditor", "PostalCodeID", "TMS.PostalCode");
            DropForeignKey("TMS.Expeditor", "ExpeditorTypeID", "TMS.ExpeditorType");
            DropIndex("TMS.UserApplication", new[] { "UserID" });
            DropIndex("TMS.UserApplication", new[] { "ApplicationID" });
            DropIndex("TMS.User", new[] { "BusinessAreaID" });
            DropIndex("TMS.User", new[] { "RoleID" });
            DropIndex("TMS.User", "User_UserName");
            DropIndex("TMS.Token", new[] { "UserID" });
            DropIndex("TMS.RoleMenu", new[] { "MenuID" });
            DropIndex("TMS.RoleMenu", new[] { "RoleID" });
            DropIndex("TMS.RoleMenuActivity", new[] { "ActivityID" });
            DropIndex("TMS.RoleMenuActivity", new[] { "RoleMenuID" });
            DropIndex("TMS.Expeditor", new[] { "ExpeditorTypeID" });
            DropIndex("TMS.Expeditor", new[] { "PostalCodeID" });
            DropColumn("TMS.Role", "IsActive");
            DropTable("TMS.UserApplication");
            DropTable("TMS.User");
            DropTable("TMS.Token");
            DropTable("TMS.RoleMenu");
            DropTable("TMS.RoleMenuActivity");
            DropTable("TMS.ExpeditorType");
            DropTable("TMS.Expeditor");
        }
    }
}
