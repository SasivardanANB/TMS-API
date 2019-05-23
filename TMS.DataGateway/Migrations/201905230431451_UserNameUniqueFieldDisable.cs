namespace TMS.DataGateway.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UserNameUniqueFieldDisable : DbMigration
    {
        public override void Up()
        {
            DropIndex("TMS.User", "User_UserName");
        }
        
        public override void Down()
        {
            CreateIndex("TMS.User", "UserName", unique: true, name: "User_UserName");
        }
    }
}
