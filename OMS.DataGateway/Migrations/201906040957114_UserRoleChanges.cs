namespace OMS.DataGateway.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UserRoleChanges : DbMigration
    {
        public override void Up()
        {
            DropIndex("OMS.Role", "Role_RoleCode");
            DropColumn("OMS.Role", "ValidFrom");
            DropColumn("OMS.Role", "ValidTo");
        }
        
        public override void Down()
        {
            AddColumn("OMS.Role", "ValidTo", c => c.DateTime(nullable: false));
            AddColumn("OMS.Role", "ValidFrom", c => c.DateTime(nullable: false));
            CreateIndex("OMS.Role", "RoleCode", unique: true, name: "Role_RoleCode");
        }
    }
}
