namespace TMS.DataGateway.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AuthorizationAPIs : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("TMS.User", "BusinessAreaID", "TMS.BusinessArea");
            DropForeignKey("TMS.User", "RoleID", "TMS.Role");
            DropIndex("TMS.BusinessArea", "BusinessArea_CompanyCodeID");
            DropIndex("TMS.User", new[] { "RoleID" });
            DropIndex("TMS.User", new[] { "BusinessAreaID" });
            CreateTable(
                "TMS.UserRoles",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        UserID = c.Int(nullable: false),
                        RoleID = c.Int(nullable: false),
                        BusinessAreaID = c.Int(nullable: false),
                        IsDelete = c.Boolean(nullable: false),
                        CreatedBy = c.String(defaultValue: "SYSTEM"),
                        CreatedTime = c.DateTime(nullable: false, defaultValueSql: "GETDATE()"),
                        LastModifiedBy = c.String(),
                        LastModifiedTime = c.DateTime(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("TMS.BusinessArea", t => t.BusinessAreaID, cascadeDelete: true)
                .ForeignKey("TMS.Role", t => t.RoleID, cascadeDelete: true)
                .ForeignKey("TMS.User", t => t.UserID, cascadeDelete: true)
                .Index(t => t.UserID)
                .Index(t => t.RoleID)
                .Index(t => t.BusinessAreaID);
            
            AddColumn("TMS.Activity", "CreatedBy", c => c.String(defaultValue: "SYSTEM"));
            AddColumn("TMS.Activity", "CreatedTime", c => c.DateTime(nullable: false, defaultValueSql: "GETDATE()"));
            AddColumn("TMS.Activity", "LastModifiedBy", c => c.String());
            AddColumn("TMS.Activity", "LastModifiedTime", c => c.DateTime());
            AddColumn("TMS.Application", "CreatedBy", c => c.String(defaultValue: "SYSTEM"));
            AddColumn("TMS.Application", "CreatedTime", c => c.DateTime(nullable: false, defaultValueSql: "GETDATE()"));
            AddColumn("TMS.Application", "LastModifiedBy", c => c.String());
            AddColumn("TMS.Application", "LastModifiedTime", c => c.DateTime());
            AddColumn("TMS.BusinessArea", "CreatedBy", c => c.String(defaultValue: "SYSTEM"));
            AddColumn("TMS.BusinessArea", "CreatedTime", c => c.DateTime(nullable: false, defaultValueSql: "GETDATE()"));
            AddColumn("TMS.BusinessArea", "LastModifiedBy", c => c.String());
            AddColumn("TMS.BusinessArea", "LastModifiedTime", c => c.DateTime());
            AddColumn("TMS.City", "CreatedBy", c => c.String(defaultValue: "SYSTEM"));
            AddColumn("TMS.City", "CreatedTime", c => c.DateTime(nullable: false, defaultValueSql: "GETDATE()"));
            AddColumn("TMS.City", "LastModifiedBy", c => c.String());
            AddColumn("TMS.City", "LastModifiedTime", c => c.DateTime());
            AddColumn("TMS.Province", "CreatedBy", c => c.String(defaultValue: "SYSTEM"));
            AddColumn("TMS.Province", "CreatedTime", c => c.DateTime(nullable: false, defaultValueSql: "GETDATE()"));
            AddColumn("TMS.Province", "LastModifiedBy", c => c.String());
            AddColumn("TMS.Province", "LastModifiedTime", c => c.DateTime());
            AddColumn("TMS.CompanyCode", "CreatedBy", c => c.String(defaultValue: "SYSTEM"));
            AddColumn("TMS.CompanyCode", "CreatedTime", c => c.DateTime(nullable: false, defaultValueSql: "GETDATE()"));
            AddColumn("TMS.CompanyCode", "LastModifiedBy", c => c.String());
            AddColumn("TMS.CompanyCode", "LastModifiedTime", c => c.DateTime());
            AddColumn("TMS.PostalCode", "CreatedBy", c => c.String(defaultValue: "SYSTEM"));
            AddColumn("TMS.PostalCode", "CreatedTime", c => c.DateTime(nullable: false, defaultValueSql: "GETDATE()"));
            AddColumn("TMS.PostalCode", "LastModifiedBy", c => c.String());
            AddColumn("TMS.PostalCode", "LastModifiedTime", c => c.DateTime());
            AddColumn("TMS.SubDistrict", "CreatedBy", c => c.String(defaultValue: "SYSTEM"));
            AddColumn("TMS.SubDistrict", "CreatedTime", c => c.DateTime(nullable: false, defaultValueSql: "GETDATE()"));
            AddColumn("TMS.SubDistrict", "LastModifiedBy", c => c.String());
            AddColumn("TMS.SubDistrict", "LastModifiedTime", c => c.DateTime());
            AddColumn("TMS.MenuActivity", "CreatedBy", c => c.String(defaultValue: "SYSTEM"));
            AddColumn("TMS.MenuActivity", "CreatedTime", c => c.DateTime(nullable: false, defaultValueSql: "GETDATE()"));
            AddColumn("TMS.MenuActivity", "LastModifiedBy", c => c.String());
            AddColumn("TMS.MenuActivity", "LastModifiedTime", c => c.DateTime());
            AddColumn("TMS.Menu", "CreatedBy", c => c.String(defaultValue: "SYSTEM"));
            AddColumn("TMS.Menu", "CreatedTime", c => c.DateTime(nullable: false, defaultValueSql: "GETDATE()"));
            AddColumn("TMS.Menu", "LastModifiedBy", c => c.String());
            AddColumn("TMS.Menu", "LastModifiedTime", c => c.DateTime());
            AddColumn("TMS.RoleMenuActivity", "CreatedBy", c => c.String());
            AddColumn("TMS.RoleMenuActivity", "CreatedTime", c => c.DateTime(nullable: false, defaultValueSql: "GETDATE()"));
            AddColumn("TMS.RoleMenuActivity", "LastModifiedBy", c => c.String());
            AddColumn("TMS.RoleMenuActivity", "LastModifiedTime", c => c.DateTime());
            AddColumn("TMS.RoleMenu", "CreatedBy", c => c.String());
            AddColumn("TMS.RoleMenu", "CreatedTime", c => c.DateTime(nullable: false, defaultValueSql: "GETDATE()"));
            AddColumn("TMS.RoleMenu", "LastModifiedBy", c => c.String());
            AddColumn("TMS.RoleMenu", "LastModifiedTime", c => c.DateTime());
            AddColumn("TMS.Role", "IsDelete", c => c.Boolean(nullable: false));
            AddColumn("TMS.Role", "CreatedBy", c => c.String());
            AddColumn("TMS.Role", "CreatedTime", c => c.DateTime(nullable: false, defaultValueSql: "GETDATE()"));
            AddColumn("TMS.Role", "LastModifiedBy", c => c.String());
            AddColumn("TMS.Role", "LastModifiedTime", c => c.DateTime());
            AddColumn("TMS.Token", "CreatedBy", c => c.String());
            AddColumn("TMS.Token", "CreatedTime", c => c.DateTime(nullable: false, defaultValueSql: "GETDATE()"));
            AddColumn("TMS.Token", "LastModifiedBy", c => c.String());
            AddColumn("TMS.Token", "LastModifiedTime", c => c.DateTime());
            AddColumn("TMS.User", "IsDelete", c => c.Boolean(nullable: false));
            AddColumn("TMS.User", "CreatedBy", c => c.String());
            AddColumn("TMS.User", "CreatedTime", c => c.DateTime(nullable: false, defaultValueSql: "GETDATE()"));
            AddColumn("TMS.User", "LastModifiedBy", c => c.String());
            AddColumn("TMS.User", "LastModifiedTime", c => c.DateTime());
            AddColumn("TMS.UserApplication", "CreatedBy", c => c.String());
            AddColumn("TMS.UserApplication", "CreatedTime", c => c.DateTime(nullable: false, defaultValueSql: "GETDATE()"));
            AddColumn("TMS.UserApplication", "LastModifiedBy", c => c.String());
            AddColumn("TMS.UserApplication", "LastModifiedTime", c => c.DateTime());
            AlterColumn("TMS.BusinessArea", "CompanyCodeID", c => c.Int());
            AlterColumn("TMS.BusinessArea", "PostalCodeID", c => c.Int());
            CreateIndex("TMS.BusinessArea", "CompanyCodeID", unique: true, name: "BusinessArea_CompanyCodeID");
            CreateIndex("TMS.BusinessArea", "PostalCodeID");
            AddForeignKey("TMS.BusinessArea", "CompanyCodeID", "TMS.CompanyCode", "ID");
            AddForeignKey("TMS.BusinessArea", "PostalCodeID", "TMS.PostalCode", "ID");
            DropColumn("TMS.User", "RoleID");
            DropColumn("TMS.User", "BusinessAreaID");
        }
        
        public override void Down()
        {
            AddColumn("TMS.User", "BusinessAreaID", c => c.Int());
            AddColumn("TMS.User", "RoleID", c => c.Int());
            DropForeignKey("TMS.UserRoles", "UserID", "TMS.User");
            DropForeignKey("TMS.UserRoles", "RoleID", "TMS.Role");
            DropForeignKey("TMS.UserRoles", "BusinessAreaID", "TMS.BusinessArea");
            DropForeignKey("TMS.BusinessArea", "PostalCodeID", "TMS.PostalCode");
            DropForeignKey("TMS.BusinessArea", "CompanyCodeID", "TMS.CompanyCode");
            DropIndex("TMS.UserRoles", new[] { "BusinessAreaID" });
            DropIndex("TMS.UserRoles", new[] { "RoleID" });
            DropIndex("TMS.UserRoles", new[] { "UserID" });
            DropIndex("TMS.BusinessArea", new[] { "PostalCodeID" });
            DropIndex("TMS.BusinessArea", "BusinessArea_CompanyCodeID");
            AlterColumn("TMS.BusinessArea", "PostalCodeID", c => c.Int(nullable: false));
            AlterColumn("TMS.BusinessArea", "CompanyCodeID", c => c.Int(nullable: false));
            DropColumn("TMS.UserApplication", "LastModifiedTime");
            DropColumn("TMS.UserApplication", "LastModifiedBy");
            DropColumn("TMS.UserApplication", "CreatedTime");
            DropColumn("TMS.UserApplication", "CreatedBy");
            DropColumn("TMS.User", "LastModifiedTime");
            DropColumn("TMS.User", "LastModifiedBy");
            DropColumn("TMS.User", "CreatedTime");
            DropColumn("TMS.User", "CreatedBy");
            DropColumn("TMS.User", "IsDelete");
            DropColumn("TMS.Token", "LastModifiedTime");
            DropColumn("TMS.Token", "LastModifiedBy");
            DropColumn("TMS.Token", "CreatedTime");
            DropColumn("TMS.Token", "CreatedBy");
            DropColumn("TMS.Role", "LastModifiedTime");
            DropColumn("TMS.Role", "LastModifiedBy");
            DropColumn("TMS.Role", "CreatedTime");
            DropColumn("TMS.Role", "CreatedBy");
            DropColumn("TMS.Role", "IsDelete");
            DropColumn("TMS.RoleMenu", "LastModifiedTime");
            DropColumn("TMS.RoleMenu", "LastModifiedBy");
            DropColumn("TMS.RoleMenu", "CreatedTime");
            DropColumn("TMS.RoleMenu", "CreatedBy");
            DropColumn("TMS.RoleMenuActivity", "LastModifiedTime");
            DropColumn("TMS.RoleMenuActivity", "LastModifiedBy");
            DropColumn("TMS.RoleMenuActivity", "CreatedTime");
            DropColumn("TMS.RoleMenuActivity", "CreatedBy");
            DropColumn("TMS.Menu", "LastModifiedTime");
            DropColumn("TMS.Menu", "LastModifiedBy");
            DropColumn("TMS.Menu", "CreatedTime");
            DropColumn("TMS.Menu", "CreatedBy");
            DropColumn("TMS.MenuActivity", "LastModifiedTime");
            DropColumn("TMS.MenuActivity", "LastModifiedBy");
            DropColumn("TMS.MenuActivity", "CreatedTime");
            DropColumn("TMS.MenuActivity", "CreatedBy");
            DropColumn("TMS.SubDistrict", "LastModifiedTime");
            DropColumn("TMS.SubDistrict", "LastModifiedBy");
            DropColumn("TMS.SubDistrict", "CreatedTime");
            DropColumn("TMS.SubDistrict", "CreatedBy");
            DropColumn("TMS.PostalCode", "LastModifiedTime");
            DropColumn("TMS.PostalCode", "LastModifiedBy");
            DropColumn("TMS.PostalCode", "CreatedTime");
            DropColumn("TMS.PostalCode", "CreatedBy");
            DropColumn("TMS.CompanyCode", "LastModifiedTime");
            DropColumn("TMS.CompanyCode", "LastModifiedBy");
            DropColumn("TMS.CompanyCode", "CreatedTime");
            DropColumn("TMS.CompanyCode", "CreatedBy");
            DropColumn("TMS.Province", "LastModifiedTime");
            DropColumn("TMS.Province", "LastModifiedBy");
            DropColumn("TMS.Province", "CreatedTime");
            DropColumn("TMS.Province", "CreatedBy");
            DropColumn("TMS.City", "LastModifiedTime");
            DropColumn("TMS.City", "LastModifiedBy");
            DropColumn("TMS.City", "CreatedTime");
            DropColumn("TMS.City", "CreatedBy");
            DropColumn("TMS.BusinessArea", "LastModifiedTime");
            DropColumn("TMS.BusinessArea", "LastModifiedBy");
            DropColumn("TMS.BusinessArea", "CreatedTime");
            DropColumn("TMS.BusinessArea", "CreatedBy");
            DropColumn("TMS.Application", "LastModifiedTime");
            DropColumn("TMS.Application", "LastModifiedBy");
            DropColumn("TMS.Application", "CreatedTime");
            DropColumn("TMS.Application", "CreatedBy");
            DropColumn("TMS.Activity", "LastModifiedTime");
            DropColumn("TMS.Activity", "LastModifiedBy");
            DropColumn("TMS.Activity", "CreatedTime");
            DropColumn("TMS.Activity", "CreatedBy");
            DropTable("TMS.UserRoles");
            CreateIndex("TMS.User", "BusinessAreaID");
            CreateIndex("TMS.User", "RoleID");
            CreateIndex("TMS.BusinessArea", "CompanyCodeID", unique: true, name: "BusinessArea_CompanyCodeID");
            AddForeignKey("TMS.User", "RoleID", "TMS.Role", "ID");
            AddForeignKey("TMS.User", "BusinessAreaID", "TMS.BusinessArea", "ID");
        }
    }
}
