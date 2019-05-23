namespace DMS.DataGateway.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UserNameUniqueFieldDisable : DbMigration
    {
        public override void Up()
        {
            DropIndex("DMS.User", "User_UserName");
        }
        
        public override void Down()
        {
            CreateIndex("DMS.User", "UserName", unique: true, name: "User_UserName");
        }
    }
}
